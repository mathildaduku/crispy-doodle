using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;


namespace NotificationService
{
    public class FollowNotificationFunction
    {
        private readonly ILogger<FollowNotificationFunction> _logger;
        private readonly IEmailService _emailService;

        public FollowNotificationFunction(ILogger<FollowNotificationFunction> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [Function("FollowNotificationFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            _emailService.SendEmail("Crispy Doodle", "merakipresets0@gmail.com", "You got a new follower", "Esther followed you.");

            return new OkObjectResult("Email sent successfully!");
        }
    }
}