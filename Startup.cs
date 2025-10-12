using System.Reflection;
using Microsoft.OpenApi.Models;
using MongoBackupBackend.Helpers;
using MongoBackupBackend.Interfaces;
using MongoBackupBackend.Jobs;
using MongoBackupBackend.Services;
using Quartz;
using Quartz.AspNetCore;
using Scalar.AspNetCore;

namespace MongoBackupBackend;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IBackupService, BackupService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder.WithOrigins("localhost")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("BackupJob");
            q.AddJob<BackupJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .StartAt(TimeHelper.GetStartTime(configuration["Settings:Time"]))
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromDays(1))
                    .RepeatForever())
            );
        });

        services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger(opt =>
        {
            opt.RouteTemplate = "openapi/{documentName}.json";
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowSpecificOrigins");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapScalarApiReference(opt =>
            {
                opt.PersistentAuthentication = true;
                opt.Title = "Mongo Backup";
                opt.Theme = ScalarTheme.BluePlanet;
                opt.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Http, ScalarClient.Http11);
            });

            endpoints.MapDefaultControllerRoute().AllowAnonymous();
            endpoints.MapSwagger();
            endpoints.MapControllers().AllowAnonymous();
        });
    }
}