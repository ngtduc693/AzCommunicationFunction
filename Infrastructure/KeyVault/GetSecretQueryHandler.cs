using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.KeyVault
{
    public class GetSecretQueryHandler : IGetSecretQueryHandler
    {
        private readonly SecretClient _secretClient;
        private readonly Dictionary<string, string> _secretCache = new();
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public GetSecretQueryHandler(AppSettings settings)
        {
            // Use DefaultAzureCredential for cloud native (Managed Identity)
            _secretClient = new SecretClient(new Uri(settings.KeyVault.VaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> HandleAsync(GetSecretQuery query)
        {
            if (query.UseCache)
            {
                await _cacheLock.WaitAsync();
                try
                {
                    if (_secretCache.TryGetValue(query.SecretName, out var cachedSecret))
                    {
                        return cachedSecret;
                    }

                    var secret = await _secretClient.GetSecretAsync(query.SecretName);
                    var secretValue = secret.Value.Value;
                    _secretCache[query.SecretName] = secretValue;
                    return secretValue;
                }
                finally
                {
                    _cacheLock.Release();
                }
            }
            else
            {
                var secret = await _secretClient.GetSecretAsync(query.SecretName);
                return secret.Value.Value;
            }
        }
    }
}
