using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Email
{
    public class SendEmailCommand
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public EmailRequest EmailRequest { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Context { get; set; } = new();
    }
}
