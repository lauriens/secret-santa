using Microsoft.AspNetCore.Mvc;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecretSantaController : ControllerBase
    {
        private readonly ILogger<SecretSantaController> logger;
        private readonly ISecretSantaService secretSantaService;
        private readonly IEmailService emailService;

        public SecretSantaController(
            ILogger<SecretSantaController> logger, 
            ISecretSantaService secretSantaService, 
            IEmailService emailService)
        {
            this.logger = logger;
            this.secretSantaService = secretSantaService;
            this.emailService = emailService;
        }

        [HttpPost("draw")]
        public async Task DrawSecretSanta([FromBody] IList<Friend> friends)
        {
            logger.LogInformation("Received request for Draw Secret Santa");
            var shuffleResult = secretSantaService.DrawSecretSata(friends);

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 10
            };

            await Parallel.ForEachAsync(shuffleResult, parallelOptions, async (friend, cancelationToken) =>
            {
                await emailService.SendEmail(friend);
            });
        }
    }
}