namespace CommunicationFunction.Core.Models
{
    public class CommunicationLog
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string MessageId { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> Details { get; set; } = [];
        public string PartitionKey => MessageType;
    }
}
