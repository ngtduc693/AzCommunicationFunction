namespace CommunicationFunction.Core.Configuration
{
    public class ServiceBusSettings
    {
        public string QueueName { get; set; } = "communication-queue";
        public string ConnectionStringName { get; set; } = "ServiceBusConnection";
    }
}
