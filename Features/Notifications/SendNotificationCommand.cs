using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Notifications
{
    public class SendNotificationCommand
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public NotificationRequest NotificationRequest { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Context { get; set; } = new();
    }
}
