using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Api.OData;
using cCoder.Mail.Models;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.EventHandlers;
using cCoder.Mail.Exposures.HostedServices;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Mail.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;
using IJsonBroker = cCoder.Mail.Brokers.IJsonBroker;
using JsonBroker = cCoder.Mail.Brokers.JsonBroker;


namespace cCoder.Mail;

public static partial class IServiceCollectionExtensions
{
    public static void AddMailWeb(
        this IServiceCollection services,
        Action<MailConfiguration> configure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredMailWeb((_, configuration) => configure?.Invoke(configuration), builder);

    public static void AddMailHostedServices(
        this IServiceCollection services,
        Action<MailConfiguration> configure = null) =>
        services.AddConfiguredMailHostedServices((_, configuration) => configure?.Invoke(configuration));

    private static void AddMail(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
    }

    private static void AddMailWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddMail();

    }

    private static void AddMailHostedServices(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddSingleton<IMailSenderHostedService, MailSenderHostedService>();
        services.AddHostedService(provider => provider.GetRequiredService<IMailSenderHostedService>());
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<MailServer>();
        services.AddEventingForType<QueuedEmail>();
        services.AddEventingForType<SentEmail>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IMailServerEventBroker, MailServerEventBroker>();
        services.AddTransient<IQueuedEmailEventBroker, QueuedEmailEventBroker>();
        services.AddTransient<ISentEmailEventBroker, SentEmailEventBroker>();
        services.AddTransient<IMailClient, MailClient>();
        services.AddTransient<IMicrosoftGraphClient, MicrosoftGraphClient>();
        services.AddTransient<IMailClientBroker, MailClientBroker>();
        services.AddTransient<IMailServerBroker, MailServerBroker>();
        services.AddTransient<IQueuedEmailBroker, QueuedEmailBroker>();
        services.AddTransient<ISentEmailBroker, SentEmailBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IMailAppExposure, MailAppExposure>();
        services.AddTransient<IMailManagerExposure, MailManagerExposure>();
        services.AddTransient<IMailMetadataTypeService, MailMetadataTypeService>();
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IMailServerService, MailServerService>();
        services.AddTransient<IMailSendingService, MailSendingService>();
        services.AddTransient<IMailReceivingService, MailReceivingService>();
        services.AddTransient<IQueuedEmailService, QueuedEmailService>();
        services.AddTransient<ISentEmailService, SentEmailService>();
        services.AddTransient<IMailServerEventService, MailServerEventService>();
        services.AddTransient<IQueuedEmailEventService, QueuedEmailEventService>();
        services.AddTransient<ISentEmailEventService, SentEmailEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IMailClientOrchestrationService, MailClientOrchestrationService>();
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddTransient<IMailServerOrchestrationService, MailServerOrchestrationService>();
        services.AddTransient<IQueuedEmailOrchestrationService, QueuedEmailOrchestrationService>();
        services.AddTransient<ISentEmailOrchestrationService, SentEmailOrchestrationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IMailEventHandlers, MailEventHandlers>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IMailServerEventProcessingService, MailServerEventProcessingService>();
        services.AddTransient<IMailServerProcessingService, MailServerProcessingService>();
        services.AddTransient<IQueuedEmailEventProcessingService, QueuedEmailEventProcessingService>();
        services.AddTransient<IQueuedEmailProcessingService, QueuedEmailProcessingService>();
        services.AddTransient<ISentEmailEventProcessingService, SentEmailEventProcessingService>();
        services.AddTransient<ISentEmailProcessingService, SentEmailProcessingService>();
    }
}
