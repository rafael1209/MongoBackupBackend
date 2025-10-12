namespace MongoBackupBackend.Interfaces;

public interface IBackupService
{
    Task<string> CreateBackupAsync();
}