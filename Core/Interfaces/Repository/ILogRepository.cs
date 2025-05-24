using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Interfaces.Repository
{
    public interface ILogRepository
    {
        Task SaveLogAsync(CommunicationLog log);
        Task<CommunicationLog?> GetLogAsync(string id);
        Task<IEnumerable<CommunicationLog>> GetLogsByMessageIdAsync(string messageId);
    }
}
