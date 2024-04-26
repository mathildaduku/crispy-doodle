using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using NotificationService.Data;
using NotificationService.Models;
using MailKit.Net.Smtp;
using NotificationService.Helpers;
using Contracts;

namespace NotificationService
{
    public class NewPostFunction
    {
        private readonly ILogger<NewPostFunction> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public NewPostFunction(ILogger<NewPostFunction> logger, ISubscriptionService subscriptionService, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _userService = userService;
            _emailService = emailService;
        }

        [Function(nameof(NewPostFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/postcreated", "notification-post-created", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a User object
                var postCreated = JsonConvert.DeserializeObject<CustomServiceBusMessage<PostCreated>>(Encoding.UTF8.GetString(message.Body.ToArray()))?.Message;

                // Fetch all subscriptions for the specified target user
                var userSubscriptions = await _subscriptionService.GetUserSubscribers(postCreated.Author);

                // Prepare a list of tasks
                var emailTasks = new List<Task>();

                // Prepare to send email notifications to all subscribers
                foreach (var subscriberUser in userSubscriptions)
                {
                    // Fetch the associated user for the subscription
                    var subscriber = await _userService.GetUserAsync(subscriberUser.SubscriberUserId);

                    if (subscriber == null)
                    {
                        _logger.LogError($"Subscriber not found for user ID: {subscriberUser.SubscriberUserId}");
                        throw new Exception($"Subscriber not found for user ID: {subscriberUser.SubscriberUserId}");
                    }

                    // Prepare to send notification to the subscriber
                    var emailTask = _emailService.SendHtmlEmailAsync(subscriber.Email, "New Post Notification", "NewPostNotification", new { subscriber.FirstName })
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                _logger.LogError($"Error sending notification to {subscriber.Email}: {t.Exception.InnerException.Message}");
                            }
                            else
                            {
                                _logger.LogInformation($"Notification sent to {subscriber.Email}");
                            }
                        });

                    // Add the task to the list
                    emailTasks.Add(emailTask);
                }

                // Wait for all email sending tasks to complete
                await Task.WhenAll(emailTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notifications: {ex.Message}");
            }

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
    /*                // send email notification to all subscribers
                    foreach (var subscriberUser in userSubscriptions)
                    {
                        // Fetch the associated user for the subscription
                        var subscriber = await _userService.GetUserAsync(subscriberUser.SubscriberUserId);

                        if (subscriber == null)
                        {
                            _logger.LogError($"Subscriber not found for user ID: {subscriberUser.SubscriberUserId}");
                            throw new Exception($"Subscriber not found for user ID: {subscriberUser.SubscriberUserId}");
                        }

                        // Send notification to the subscriber
                        try
                        {
                            await _emailService.SendHtmlEmailAsync(subscriber.Email, "New Post Notification", "NewPostNotification", new { subscriber.FirstName });

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error sending notification to {subscriber.Email}: {ex.Message}");
                        }
                        // await SendNotification(subscriber, newPostData);

                        _logger.LogInformation($"Notification sent to {subscriber.Email}");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending notifications: {ex.Message}");
                }

                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }

        }*/
}
