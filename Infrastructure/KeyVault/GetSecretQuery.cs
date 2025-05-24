using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.KeyVault
{
    public class GetSecretQuery
    {
        public string SecretName { get; set; } = string.Empty;
        public bool UseCache { get; set; } = true;
    }
}
