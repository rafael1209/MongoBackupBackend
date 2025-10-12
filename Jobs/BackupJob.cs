using MongoBackupBackend.Interfaces;
using Quartz;

namespace MongoBackupBackend.Jobs;

[DisallowConcurrentExecution]
public class BackupJob(IBackupService backupService) : IJob
{
    public async Task Execute(IJobExecutionContext context) => 
        await backupService.CreateBackupAsync();
}