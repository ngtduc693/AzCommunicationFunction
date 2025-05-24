namespace CommunicationFunction.Core.Models
{
    public class ServiceBusMessage
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string MessageType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public EmailRequest? EmailRequest { get; set; }
        public NotificationRequest? NotificationRequest { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = [];
    }
}
