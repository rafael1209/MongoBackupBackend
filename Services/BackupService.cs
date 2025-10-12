using MongoBackupBackend.Interfaces;
using MongoDB.Driver;

namespace MongoBackupBackend.Services
{
    public class BackupService(ILogger<BackupService> logger, IConfiguration config) : IBackupService
    {
        private readonly string _localUri = config["MongoDB:LocalConnectionString"]!;
        private readonly List<string> _remoteUris = config.GetSection("MongoDB:RemoteConnectionStrings").Get<List<string>>() ?? [];

        public async Task<string> CreateBackupAsync()
        {
            logger.LogInformation("Starting MongoDB backup...");

            var localClient = new MongoClient(_localUri);
            var localDbList = await localClient.ListDatabaseNamesAsync();
            var databases = await localDbList.ToListAsync();

            foreach (var dbName in databases.Where(dbName => dbName is not ("admin" or "local" or "config")))
            {
                logger.LogInformation("Backing up database: {DbName}", dbName);

                var sourceDb = localClient.GetDatabase(dbName);
                var collections = await sourceDb.ListCollectionNamesAsync();
                var colNames = await collections.ToListAsync();

                foreach (var remoteUri in _remoteUris)
                {
                    var remoteClient = new MongoClient(remoteUri);
                    var targetDb = remoteClient.GetDatabase(dbName);

                    logger.LogInformation("Copying to {RemoteUri}", remoteUri);

                    foreach (var colName in colNames)
                    {
                        var sourceCollection = sourceDb.GetCollection<dynamic>(colName);
                        var targetCollection = targetDb.GetCollection<dynamic>(colName);

                        logger.LogInformation(" - Collection: {ColName}", colName);

                        await targetCollection.DeleteManyAsync(FilterDefinition<dynamic>.Empty);

                        var docs = await sourceCollection.Find(FilterDefinition<dynamic>.Empty).ToListAsync();
                        if (docs.Count > 0)
                            await targetCollection.InsertManyAsync(docs);
                    }
                }
            }

            var message = $"Backup completed successfully at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            logger.LogInformation(message);
            return message;
        }
    }
}
