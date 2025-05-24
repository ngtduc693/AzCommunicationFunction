using CommunicationFunction.Features.Notifications;
using CommunicationFunction.Core.Interfaces.CommandHandler;
using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationFunction.Features.Logging;
using CommunicationFunction.Features.Email;

namespace CommunicationFunction
{
    /// <summary>
    /// Handles messages from Service Bus, invokes corresponding handlers to send email, notification, and log communication.
    /// </summary>
    public class MessageProcessor
    {
        private readonly ISendEmailCommandHandler _emailHandler;
        private readonly ISendNotificationCommandHandler _notificationHandler;
        private readonly ILogCommunicationCommandHandler _logHandler;

        /// <summary>
        /// Initializes MessageProcessor with required handlers.
        /// </summary>
        public MessageProcessor(
            ISendEmailCommandHandler emailHandler,
            ISendNotificationCommandHandler notificationHandler,
            ILogCommunicationCommandHandler logHandler)
        {
            _emailHandler = emailHandler;
            _notificationHandler = notificationHandler;
            _logHandler = logHandler;
        }

        /// <summary>
        /// Processes a message received from Service Bus.
        /// </summary>
        /// <param name="message">Message from Service Bus</param>
        /// <returns>Processing result</returns>
        public async Task<ProcessingResult> ProcessMessageAsync(ServiceBusMessage message)
        {
            var overallResult = new ProcessingResult
            {
                MessageId = message.MessageId,
                Success = true
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Log message received
                await LogMessageAsync(message, "Received", null);

                ProcessingResult? result = null;

                switch (message.MessageType.ToLower())
                {
                    case "email":
                        if (message.EmailRequest != null)
                        {
                            result = await _emailHandler.HandleAsync(new SendEmailCommand
                            {
                                EmailRequest = message.EmailRequest,
                                Context = message.AdditionalData
                            });
                        }
                        break;

                    case "notification":
                        if (message.NotificationRequest != null)
                        {
                            result = await _notificationHandler.HandleAsync(new SendNotificationCommand
                            {
                                NotificationRequest = message.NotificationRequest,
                                Context = message.AdditionalData
                            });
                        }
                        break;

                    default:
                        overallResult.Success = false;
                        overallResult.ErrorMessage = $"Unknown message type: {message.MessageType}";
                        break;
                }

                if (result != null)
                {
                    overallResult.Success = result.Success;
                    overallResult.ErrorMessage = result.ErrorMessage;
                    overallResult.Metadata = result.Metadata;
                }

                // Log completion
                await LogMessageAsync(message,
                    overallResult.Success ? "Completed" : "Failed",
                    overallResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                overallResult.Success = false;
                overallResult.ErrorMessage = ex.Message;
                await LogMessageAsync(message, "Error", ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                overallResult.ProcessingDuration = stopwatch.Elapsed;
            }

            return overallResult;
        }

        /// <summary>
        /// Logs the processing status of a message.
        /// </summary>
        private async Task LogMessageAsync(ServiceBusMessage message, string status, string? errorMessage)
        {
            try
            {
                await _logHandler.HandleAsync(new LogCommunicationCommand
                {
                    Log = new CommunicationLog
                    {
                        MessageId = message.MessageId,
                        MessageType = message.MessageType,
                        Status = status,
                        ErrorMessage = errorMessage,
                        Details = new Dictionary<string, object>
                        {
                            ["OriginalTimestamp"] = message.Timestamp,
                            ["HasEmailRequest"] = message.EmailRequest != null,
                            ["HasNotificationRequest"] = message.NotificationRequest != null,
                            ["AdditionalDataCount"] = message.AdditionalData.Count
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Log failure shouldn't break the main process
                System.Diagnostics.Debug.WriteLine($"Failed to log message: {ex.Message}");
            }
        }
    }
}
