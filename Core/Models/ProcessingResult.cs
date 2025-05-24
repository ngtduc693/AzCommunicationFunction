namespace CommunicationFunction.Core.Models
{
    public class ProcessingResult
    {
        public string MessageId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan ProcessingDuration { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
