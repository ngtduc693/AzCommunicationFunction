using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.CommandHandler;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using CommunicationFunction.Core.Models;
using CommunicationFunction.Infrastructure.KeyVault;
using CommunicationFunction.Infrastructure.Templates;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Notifications
{
    /// <summary>
    /// Handles sending notifications via Azure Notification Hubs.
    /// </summary>
    public class SendNotificationCommandHandler : ISendNotificationCommandHandler
    {
        private readonly IGetSecretsQueryHandler _secretsQueryHandler;
        private readonly IProcessTemplateQueryHandler _templateQueryHandler;
        private readonly AppSettings _appSettings;
        private NotificationHubClient? _hubClient;

        public SendNotificationCommandHandler(
            IGetSecretsQueryHandler secretsQueryHandler,
            IProcessTemplateQueryHandler templateQueryHandler,
            AppSettings appSettings)
        {
            _secretsQueryHandler = secretsQueryHandler;
            _templateQueryHandler = templateQueryHandler;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Handles sending notification based on the received command.
        /// </summary>
        public async Task<ProcessingResult> HandleAsync(SendNotificationCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new ProcessingResult { MessageId = command.CommandId };

            try
            {
                var client = await GetNotificationHubClientAsync();
                var (title, body) = await PrepareNotificationContentAsync(command.NotificationRequest);

                NotificationOutcome? outcome = null;

                if (command.NotificationRequest.Tags.Any())
                {
                    var tagExpression = string.Join(" || ", command.NotificationRequest.Tags);
                    outcome = await client.SendTemplateNotificationAsync(
                        new Dictionary<string, string>
                        {
                            ["title"] = title,
                            ["body"] = body,
                            ["data"] = JsonConvert.SerializeObject(command.NotificationRequest.Data)
                        },
                        tagExpression);
                }
                else
                {
                    outcome = await client.SendTemplateNotificationAsync(
                        new Dictionary<string, string>
                        {
                            ["title"] = title,
                            ["body"] = body,
                            ["data"] = JsonConvert.SerializeObject(command.NotificationRequest.Data)
                        });
                }

                result.Success = true;
                result.Metadata["NotificationId"] = outcome?.NotificationId ?? "broadcast";
                result.Metadata["State"] = outcome?.State.ToString() ?? "sent";
                result.Metadata["HubName"] = _appSettings.Notification.HubName;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ProcessingDuration = stopwatch.Elapsed;
            }

            return result;
        }

        private async Task<NotificationHubClient> GetNotificationHubClientAsync()
        {
            if (_hubClient == null)
            {
                var secrets = await _secretsQueryHandler.HandleAsync(new GetSecretsQuery
                {
                    SecretNames = new[] { SecretNames.NotificationHubConnectionString }
                });

                _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                    secrets[SecretNames.NotificationHubConnectionString],
                    _appSettings.Notification.HubName);
            }
            return _hubClient;
        }

        private async Task<(string title, string body)> PrepareNotificationContentAsync(NotificationRequest request)
        {
            if (!string.IsNullOrEmpty(request.TemplateId))
            {
                var templateContent = await _templateQueryHandler.HandleAsync(new ProcessTemplateQuery
                {
                    TemplateId = request.TemplateId,
                    TemplateType = "Notification",
                    Data = request.TemplateData
                });

                // Parse template content for title and body
                var parts = templateContent.Split('|', 2);
                return parts.Length == 2 ? (parts[0], parts[1]) : (request.Title, templateContent);
            }
            return (request.Title, request.Body);
        }
    }
}
