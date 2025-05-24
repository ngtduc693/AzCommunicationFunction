using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;
using ServiceBusMessage = CommunicationFunction.Core.Models.ServiceBusMessage;

namespace CommunicationFunction.Functions
{
    /// <summary>
    /// Azure Function that processes messages from Service Bus and delegates to MessageProcessor.
    /// </summary>
    public class CommunicationFunction
    {
        private readonly ILogger<CommunicationFunction> _logger;
        private readonly MessageProcessor _messageProcessor;

        public CommunicationFunction(ILogger<CommunicationFunction> logger, MessageProcessor messageProcessor)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Function to process messages from Service Bus trigger.
        /// </summary>
        /// <param name="message">Message received from Service Bus</param>
        /// <param name="messageActions">Actions for the message</param>
        [Function("ProcessCommunicationMessage")]
        public async Task ProcessMessage(
            [ServiceBusTrigger("communication-queue", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Processing message: {MessageId}", message.MessageId);

            try
            {
                var serviceBusMessage = JsonSerializer.Deserialize<ServiceBusMessage>(message.Body.ToString());
                if (serviceBusMessage == null)
                {
                    _logger.LogError("Failed to deserialize message: {MessageId}", message.MessageId);
                    await messageActions.DeadLetterMessageAsync(
                        message,
                        new Dictionary<string, object>
                        {
                            { "DeadLetterReason", "DeserializationFailed" },
                            { "DeadLetterErrorDescription", "Unable to deserialize message body" }
                        });
                    return;
                }

                var result = await _messageProcessor.ProcessMessageAsync(serviceBusMessage);

                if (result.Success)
                {
                    _logger.LogInformation("Successfully processed message: {MessageId} in {Duration}ms",
                        message.MessageId, result.ProcessingDuration.TotalMilliseconds);
                    await messageActions.CompleteMessageAsync(message);
                }
                else
                {
                    _logger.LogError("Failed to process message: {MessageId} - {Error}",
                        message.MessageId, result.ErrorMessage);
                    await messageActions.DeadLetterMessageAsync(
                        message,
                        new Dictionary<string, object>
                        {
                            { "DeadLetterReason", "ProcessingFailed" },
                            { "DeadLetterErrorDescription", result.ErrorMessage ?? string.Empty }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing message: {MessageId}", message.MessageId);
                await messageActions.DeadLetterMessageAsync(
                    message,
                    new Dictionary<string, object>
                    {
                        { "DeadLetterReason", "UnhandledException" },
                        { "DeadLetterErrorDescription", ex.Message }
                    });
            }
        }
    }
}
