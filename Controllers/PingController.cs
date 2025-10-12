using Microsoft.AspNetCore.Mvc;

namespace MongoBackupBackend.Controllers
{
    /// <summary>
    /// Provides server health check functionality.
    /// </summary>
    [ApiController]
    [Route("api/v1/ping")]
    public partial class PingController(ILogger<PingController> logger) : ControllerBase
    {
        /// <summary>
        /// Checks if the server is running.
        /// </summary>
        /// <remarks>
        /// Returns a simple "pong" response if server is operational.
        /// </remarks>
        /// <returns>
        /// Simple string response: "pong".
        /// </returns>
        /// <response code="200">Server is online and responds with "pong".</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult PingServer()
        {
            LogPingReceivedAtTime(logger, DateTime.UtcNow);
            return Ok("pong");
        }

        [LoggerMessage(LogLevel.Information, "Ping received at {time}")]
        static partial void LogPingReceivedAtTime(ILogger<PingController> logger, DateTime time);
    }
}