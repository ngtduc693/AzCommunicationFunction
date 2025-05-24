using Azure.Communication.Email;
using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.CommandHandler;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using CommunicationFunction.Core.Models;
using CommunicationFunction.Infrastructure.KeyVault;
using CommunicationFunction.Infrastructure.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Email
{
    /// <summary>
    /// Handles sending emails via Azure Communication Services.
    /// </summary>
    public class SendEmailCommandHandler : ISendEmailCommandHandler
    {
        private readonly IGetSecretQueryHandler _secretQueryHandler;
        private readonly IProcessTemplateQueryHandler _templateQueryHandler;
        private readonly AppSettings _appSettings;
        private EmailClient? _emailClient;

        public SendEmailCommandHandler(
            IGetSecretQueryHandler secretQueryHandler,
            IProcessTemplateQueryHandler templateQueryHandler,
            AppSettings appSettings)
        {
            _secretQueryHandler = secretQueryHandler;
            _templateQueryHandler = templateQueryHandler;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Handles sending email based on the received command.
        /// </summary>
        public async Task<ProcessingResult> HandleAsync(SendEmailCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new ProcessingResult { MessageId = command.CommandId };

            try
            {
                var client = await GetEmailClientAsync();
                var emailContent = await PrepareEmailContentAsync(command.EmailRequest);

                var emailMessage = new EmailMessage(
                    senderAddress: _appSettings.Email.SenderAddress,
                    content: emailContent,
                    recipients: new EmailRecipients(new List<EmailAddress>
                    {
                        new(command.EmailRequest.ToEmail, command.EmailRequest.ToName)
                    }));

                foreach (var attachment in command.EmailRequest.Attachments)
                {
                    emailMessage.Attachments.Add(new Azure.Communication.Email.EmailAttachment(
                        attachment.FileName,
                        attachment.ContentType,
                        BinaryData.FromBytes(attachment.Content)));
                }

                var emailResult = await client.SendAsync(Azure.WaitUntil.Started, emailMessage);

                result.Success = true;
                result.Metadata["OperationId"] = emailResult.Id;
                if (emailResult.HasValue && emailResult.Value != null)
                {
                    result.Metadata["Status"] = emailResult.Value.Status.ToString();
                }
                result.Metadata["SenderAddress"] = _appSettings.Email.SenderAddress;
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

        private async Task<EmailClient> GetEmailClientAsync()
        {
            if (_emailClient == null)
            {
                var connectionString = await _secretQueryHandler.HandleAsync(
                    new GetSecretQuery { SecretName = SecretNames.CommunicationServiceConnectionString });
                _emailClient = new EmailClient(connectionString);
            }
            return _emailClient;
        }

        private async Task<EmailContent> PrepareEmailContentAsync(EmailRequest request)
        {
            string subject = request.Subject;
            string htmlContent = request.HtmlContent ?? string.Empty;
            string plainTextContent = request.PlainTextContent ?? string.Empty;

            if (!string.IsNullOrEmpty(request.TemplateId))
            {
                htmlContent = await _templateQueryHandler.HandleAsync(new ProcessTemplateQuery
                {
                    TemplateId = request.TemplateId,
                    TemplateType = "Email",
                    Data = request.TemplateData
                });

                if (string.IsNullOrEmpty(plainTextContent))
                {
                    plainTextContent = StripHtmlTags(htmlContent);
                }
            }

            return new EmailContent(subject)
            {
                Html = htmlContent,
                PlainText = plainTextContent
            };
        }

        private static string StripHtmlTags(string html)
        {
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
