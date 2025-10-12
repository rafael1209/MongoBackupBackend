using MongoBackupBackend.Interfaces;
using MongoDB.Driver;

namespace MongoBackupBackend.Services
{
    public class BackupService : IBackupService
    {
        private readonly ILogger<BackupService> _logger;
        private readonly IConfiguration _config;
        private readonly string _localUri;
        private readonly string _remoteUri;

        public BackupService(ILogger<BackupService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _localUri = _config["MongoDB:LocalConnectionString"]!;
            _remoteUri = _config["MongoDB:RemoteConnectionString"]!;
        }

        public async Task<string> CreateBackupAsync()
        {
            _logger.LogInformation("Starting MongoDB backup...");

            var localClient = new MongoClient(_localUri);
            var remoteClient = new MongoClient(_remoteUri);

            var localDbList = await localClient.ListDatabaseNamesAsync();
            var databases = await localDbList.ToListAsync();

            foreach (var dbName in databases)
            {
                if (dbName is "admin" or "local" or "config")
                    continue;

                var sourceDb = localClient.GetDatabase(dbName);
                var targetDb = remoteClient.GetDatabase(dbName);

                _logger.LogInformation("Backing up database: {DbName}", dbName);

                var collections = await sourceDb.ListCollectionNamesAsync();
                var colNames = await collections.ToListAsync();

                foreach (var colName in colNames)
                {
                    var sourceCollection = sourceDb.GetCollection<dynamic>(colName);
                    var targetCollection = targetDb.GetCollection<dynamic>(colName);

                    _logger.LogInformation(" - Collection: {ColName}", colName);

                    await targetCollection.DeleteManyAsync(FilterDefinition<dynamic>.Empty);

                    var docs = await sourceCollection.Find(FilterDefinition<dynamic>.Empty).ToListAsync();
                    if (docs.Count > 0)
                        await targetCollection.InsertManyAsync(docs);
                }
            }

            var message = $"Backup completed successfully at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            _logger.LogInformation(message);

            return message;
        }
    }
}
