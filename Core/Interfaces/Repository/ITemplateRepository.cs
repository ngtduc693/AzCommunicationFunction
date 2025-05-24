using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Interfaces.Repository
{
    public interface ITemplateRepository
    {
        Task<string?> GetTemplateAsync(string templateId, string templateType);
        Task SaveTemplateAsync(string templateId, string templateType, string content);
        Task<bool> TemplateExistsAsync(string templateId, string templateType);
    }
}
