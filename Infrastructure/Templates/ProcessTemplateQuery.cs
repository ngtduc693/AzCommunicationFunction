using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.Templates
{
    public class ProcessTemplateQuery
    {
        public string TemplateId { get; set; } = string.Empty;
        public string TemplateType { get; set; } = string.Empty; // Email, Notification
        public Dictionary<string, object> Data { get; set; } = new();
    }
}
