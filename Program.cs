using CommunicationFunction;
using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.CommandHandler;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using CommunicationFunction.Core.Interfaces.Repository;
using CommunicationFunction.Features.Email;
using CommunicationFunction.Features.Logging;
using CommunicationFunction.Features.Notifications;
using CommunicationFunction.Infrastructure.KeyVault;
using CommunicationFunction.Infrastructure.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Configuration
        IConfiguration configuration = context.Configuration;
        AppSettings appSettings = configuration.Get<AppSettings>() ?? new AppSettings();
        _ = services.AddSingleton(appSettings);
        _ = services.AddSingleton(appSettings.CosmosDb);

        // Query Handlers
        services.AddSingleton<IGetSecretQueryHandler, GetSecretQueryHandler>();
        services.AddSingleton<IGetSecretsQueryHandler, GetSecretsQueryHandler>();
        services.AddSingleton<IProcessTemplateQueryHandler, ProcessTemplateQueryHandler>();

        // Repositories
        services.AddSingleton<ILogRepository, LogRepository>();
        services.AddSingleton<ITemplateRepository, TemplateRepository>();

        // Command Handlers
        services.AddSingleton<ISendEmailCommandHandler, SendEmailCommandHandler>();
        services.AddSingleton<ISendNotificationCommandHandler, SendNotificationCommandHandler>();
        services.AddSingleton<ILogCommunicationCommandHandler, LogCommunicationCommandHandler>();

        // Message Processor
        services.AddSingleton<MessageProcessor>();

        // Configure Logging
        //services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
