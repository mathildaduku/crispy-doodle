// using MailKit.Net.Smtp;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.Logging;
// using MimeKit;

// namespace DemoNamespace
// {
//     public class FollowNotificationFunction
//     {
//         private readonly ILogger<FollowNotificationFunction> _logger;

//         public FollowNotificationFunction(ILogger<FollowNotificationFunction> logger)
//         {
//             _logger = logger;
//         }

//         [Function("FollowNotificationFunction")]
//         public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
//         {
//             _logger.LogInformation("C# HTTP trigger function processed a request.");

//             var message = new MimeMessage();
//             message.From.Add(new MailboxAddress("Crispy Doodle", "myemail@gmail.com"));
//             message.To.Add(new MailboxAddress("Test User", "myemail@gmail.com"));
//             message.Subject = "You got a new follower";
//             message.Body = new TextPart("plain")
//             {
//                 Text = "Esther followed you."
//             };

//             using (var client = new SmtpClient())
//             {
//                 client.Connect("smtp.gmail.com", 465, true);
//                 client.Authenticate("myemail@gmail.com", "myapppassword");
//                 client.Send(message);
//                 client.Disconnect(true);
//             }

//             return new OkObjectResult("Email sent successfully!");
//         }
//     }
// }

