using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Configuration
{
    public class EmailSettings
    {
        public string SenderAddress { get; set; } = "noreply@yourdomain.com";
        public string SenderName { get; set; } = "Communication Service";
        public int RetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
    }
}
