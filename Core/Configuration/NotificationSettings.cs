using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Configuration
{
    public class NotificationSettings
    {
        public string HubName { get; set; } = "communication-hub";
        public int RetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
        public Dictionary<string, string> DefaultTags { get; set; } = new();
    }
}
