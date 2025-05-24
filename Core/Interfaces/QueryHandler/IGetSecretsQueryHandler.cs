using CommunicationFunction.Infrastructure.KeyVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Interfaces.QueryHandler
{
    public interface IGetSecretsQueryHandler : IQueryHandler<GetSecretsQuery, Dictionary<string, string>> { }
}
