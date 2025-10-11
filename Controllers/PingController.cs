using Microsoft.AspNetCore.Mvc;

namespace MongoBackupBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PingController(ILogger<PingController> logger) : ControllerBase
    {
        /// <summary>
        /// Проверка работы сервера.
        /// </summary>
        /// <returns>Простой ответ "pong"</returns>
        /// <response code="200">Сервер работает и возвращает pong</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult PingServer()
        {
            logger.LogInformation("Ping received at {Time}", DateTime.UtcNow);
            return Ok("pong");
        }
    }
}