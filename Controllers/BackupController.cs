using Microsoft.AspNetCore.Mvc;
using MongoBackupBackend.Interfaces;

namespace MongoBackupBackend.Controllers
{
    /// <summary>
    /// API controller for managing MongoDB database backups.
    /// Provides an endpoint to trigger the backup process via IBackupService.
    /// </summary>
    [ApiController]
    [Route("api/v1/backup")]
    public class BackupController : Controller
    {
        private readonly ILogger<BackupController> _logger;
        private readonly IBackupService _backupService;

        /// <summary>
        /// Constructs a new instance of the <see cref="BackupController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging controller events.</param>
        /// <param name="backupService">Service responsible for executing the backup logic.</param>
        public BackupController(ILogger<BackupController> logger, IBackupService backupService)
        {
            _logger = logger;
            _backupService = backupService;
        }

        /// <summary>
        /// Creates a new MongoDB backup by invoking the backup service.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/>:
        /// - 200 OK if the backup was created successfully
        /// - 400 Bad Request if the backup process fails
        /// </returns>
        /// <remarks>
        /// This endpoint does not require a request body.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                await _backupService.CreateBackupAsync();
                return Ok(new { message = "Backup created." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating backup.");
                return BadRequest(new { error = "Failed to create backup." });
            }
        }
    }
}