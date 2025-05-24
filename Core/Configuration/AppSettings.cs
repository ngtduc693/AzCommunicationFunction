namespace CommunicationFunction.Core.Configuration
{
    public class AppSettings
    {
        public KeyVaultSettings KeyVault { get; set; } = new();
        public ServiceBusSettings ServiceBus { get; set; } = new();
        public CosmosDbSettings CosmosDb { get; set; } = new();
        public EmailSettings Email { get; set; } = new();
        public NotificationSettings Notification { get; set; } = new();
        public TemplateSettings Templates { get; set; } = new();
    }
}
