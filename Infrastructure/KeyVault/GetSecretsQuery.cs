using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.KeyVault
{
    public class GetSecretsQuery
    {
        public string[] SecretNames { get; set; } = Array.Empty<string>();
        public bool UseCache { get; set; } = true;
    }
}
