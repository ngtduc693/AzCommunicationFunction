using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using CommunicationFunction.Core.Interfaces.Repository;
using CommunicationFunction.Core.Models;
using CommunicationFunction.Infrastructure.KeyVault;
using Microsoft.Azure.Cosmos;

namespace CommunicationFunction.Features.Logging
{
    /// <summary>
    /// Repository for working with communication log data in CosmosDB.
    /// </summary>
    public class LogRepository : ILogRepository
    {
        private readonly IGetSecretQueryHandler _secretQueryHandler;
        private readonly AppSettings _appSettings;
        private CosmosClient? _cosmosClient;
        private Container? _container;

        public LogRepository(IGetSecretQueryHandler secretQueryHandler, AppSettings appSettings)
        {
            _secretQueryHandler = secretQueryHandler;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Saves a communication log to CosmosDB.
        /// </summary>
        public async Task SaveLogAsync(CommunicationLog log)
        {
            var container = await GetContainerAsync();
            await container.CreateItemAsync(log, new PartitionKey(log.PartitionKey));
        }

        /// <summary>
        /// Gets a log by id.
        /// </summary>
        public async Task<CommunicationLog?> GetLogAsync(string id)
        {
            var container = await GetContainerAsync();
            try
            {
                var response = await container.ReadItemAsync<CommunicationLog>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a list of logs by MessageId.
        /// </summary>
        public async Task<IEnumerable<CommunicationLog>> GetLogsByMessageIdAsync(string messageId)
        {
            var container = await GetContainerAsync();
            var query = new QueryDefinition("SELECT * FROM c WHERE c.MessageId = @messageId")
                .WithParameter("@messageId", messageId);

            var iterator = container.GetItemQueryIterator<CommunicationLog>(query);
            var results = new List<CommunicationLog>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        private async Task<Container> GetContainerAsync()
        {
            if (_container == null)
            {
                var connectionString = await _secretQueryHandler.HandleAsync(
                    new GetSecretQuery { SecretName = SecretNames.CosmosDbConnectionString });

                _cosmosClient = new CosmosClient(connectionString);
                var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_appSettings.CosmosDb.DatabaseName);
                _container = await database.Database.CreateContainerIfNotExistsAsync(
                    _appSettings.CosmosDb.ContainerName,
                    _appSettings.CosmosDb.PartitionKeyPath);
            }
            return _container;
        }
    }
}
