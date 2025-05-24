using CommunicationFunction.Core.Interfaces.QueryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.KeyVault
{
    public class GetSecretsQueryHandler : IGetSecretsQueryHandler
    {
        private readonly IGetSecretQueryHandler _getSecretHandler;

        public GetSecretsQueryHandler(IGetSecretQueryHandler getSecretHandler)
        {
            _getSecretHandler = getSecretHandler;
        }

        public async Task<Dictionary<string, string>> HandleAsync(GetSecretsQuery query)
        {
            var secrets = new Dictionary<string, string>();
            var tasks = query.SecretNames.Select(async name => new
            {
                Name = name,
                Value = await _getSecretHandler.HandleAsync(new GetSecretQuery
                {
                    SecretName = name,
                    UseCache = query.UseCache
                })
            });

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                secrets[result.Name] = result.Value;
            }

            return secrets;
        }
    }
}
