using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Configuration
{
    public class TemplateSettings
    {
        public string TemplateStorageType { get; set; } = "InMemory"; // InMemory, File, Database
        public string TemplateBasePath { get; set; } = "Templates";
        public bool CacheTemplates { get; set; } = true;
        public int CacheExpirationMinutes { get; set; } = 60;
    }
}
