namespace CommunicationFunction.Core.Configuration
{
    public class CosmosDbSettings
    {
        public string DatabaseName { get; set; } = "CommunicationDb";
        public string ContainerName { get; set; } = "Logs";
        public string PartitionKeyPath { get; set; } = "/PartitionKey";
    }
}
