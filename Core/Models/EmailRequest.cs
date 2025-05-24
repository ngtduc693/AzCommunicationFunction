using System.ComponentModel.DataAnnotations;

namespace CommunicationFunction.Core.Models
{
    public class EmailRequest
    {
        [Required]
        public string ToEmail { get; set; } = string.Empty;
        public string? ToName { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        public string? PlainTextContent { get; set; }
        public string? HtmlContent { get; set; }
        public string? TemplateId { get; set; }
        public Dictionary<string, object> TemplateData { get; set; } = [];
        public List<EmailAttachment> Attachments { get; set; } = [];
    }
}
