using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Logging
{
    public class LogCommunicationCommand
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public CommunicationLog Log { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
