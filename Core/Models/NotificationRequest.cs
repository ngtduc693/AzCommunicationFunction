using System.ComponentModel.DataAnnotations;

namespace CommunicationFunction.Core.Models
{
    public class NotificationRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Body { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = [];
        public Dictionary<string, string> Data { get; set; } = [];
        public string? TemplateId { get; set; }
        public Dictionary<string, object> TemplateData { get; set; } = [];
    }
}
