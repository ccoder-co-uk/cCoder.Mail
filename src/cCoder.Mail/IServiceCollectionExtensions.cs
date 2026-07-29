// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.OData;
using cCoder.Mail.Models;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Brokers;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.EventHandlers;
using cCoder.Mail.Exposures.HostedServices;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Aggregations;
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

public static class IServiceCollectionExtensions
{
    public static void AddMail(
        this IServiceCollection services,
        Action<MailConfiguration> newMailConfiguration = null)
    {
        MailConfiguration configuration = new();
        newMailConfiguration?.Invoke(obj: configuration);
        services.AddMail(configuration: configuration);
    }

    public static void AddMail(
        this IServiceCollection services,
        MailConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.RegisterConfiguration(configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddMailWeb(
        this IServiceCollection services,
        Action<MailConfiguration> newMailConfiguration = null,
        ODataConventionModelBuilder builder = null)
    {
        MailConfiguration configuration = new();
        newMailConfiguration?.Invoke(obj: configuration);
        services.AddMailWeb(configuration: configuration, builder: builder);
    }

    public static void AddMailWeb(
        this IServiceCollection services,
        MailConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.RegisterConfiguration(configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddConfiguredApi(
            newMailConfiguration: configuration,
            documentName: "Mail",
            configureModel: static modelBuilder =>
                modelBuilder.ConfigureMailApiModel(),
            builder: builder);
    }

    public static void AddMailHostedServices(
        this IServiceCollection services,
        Action<MailConfiguration> newMailConfiguration = null)
    {
        MailConfiguration configuration = new();
        newMailConfiguration?.Invoke(obj: configuration);
        services.AddMailHostedServices(configuration: configuration);
    }

    public static void AddMailHostedServices(
        this IServiceCollection services,
        MailConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.RegisterConfiguration(configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddHostedServiceExposures();
    }

    private static void AddHostedServiceExposures(
        this IServiceCollection services)
    {
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddSingleton<IMailSenderHostedService, MailSenderHostedService>();
        services.AddHostedService(
            implementationFactory: provider =>
                provider.GetRequiredService<IMailSenderHostedService>());
        services.AddSingleton<IMailReceiverHostedService, MailReceiverHostedService>();
        services.AddHostedService(
            implementationFactory: provider =>
                provider.GetRequiredService<IMailReceiverHostedService>());
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<MailServer>();
        services.AddEventingForType<MailSender>();
        services.AddEventingForType<MailReceiver>();
        services.AddEventingForType<QueuedEmail>();
        services.AddEventingForType<SentEmail>();
        services.AddEventingForType<ReceivedEmail>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IAuthInfoBroker, AuthInfoBroker>();
        services.AddSingleton<IMailConfigurationExposure, MailConfigurationExposure>();
        services.AddTransient<IMailServerEventBroker, MailServerEventBroker>();
        services.AddTransient<IQueuedEmailEventBroker, QueuedEmailEventBroker>();
        services.AddTransient<ISentEmailEventBroker, SentEmailEventBroker>();
        services.AddTransient<IMailProviderCatalog, MailProviderCatalog>();
        services.AddTransient<IMailSenderClientBroker, MailSenderClientBroker>();
        services.AddTransient<IMailReceiverClientBroker, MailReceiverClientBroker>();
        services.AddTransient<IMailServerBroker, MailServerBroker>();
        services.AddTransient<IMailSenderBroker, MailSenderBroker>();
        services.AddTransient<IMailReceiverBroker, MailReceiverBroker>();
        services.AddTransient<IQueuedEmailBroker, QueuedEmailBroker>();
        services.AddTransient<ISentEmailBroker, SentEmailBroker>();
        services.AddTransient<IReceivedEmailBroker, ReceivedEmailBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IMailAppExposure, MailAppExposure>();
        services.AddTransient<IMailManagerExposure, MailManagerExposure>();
        services.AddTransient<IMailMetadataTypeService, MailMetadataTypeService>();
        services.AddTransient<
            Services.Foundations.Events.IEventHandlerService,
            Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IMailServerService, MailServerService>();
        services.AddTransient<IMailSenderService, MailSenderService>();
        services.AddTransient<IMailReceiverService, MailReceiverService>();
        services.AddTransient<IMailSendingService, MailSendingService>();
        services.AddTransient<IMailReceivingService, MailReceivingService>();
        services.AddTransient<IQueuedEmailService, QueuedEmailService>();
        services.AddTransient<ISentEmailService, SentEmailService>();
        services.AddTransient<IReceivedEmailService, ReceivedEmailService>();
        services.AddTransient<IMailServerEventService, MailServerEventService>();
        services.AddTransient<IQueuedEmailEventService, QueuedEmailEventService>();
        services.AddTransient<ISentEmailEventService, SentEmailEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppAggregationService, AppAggregationService>();
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddTransient<IMailReceiverOrchestrationService, MailReceiverOrchestrationService>();
        services.AddTransient<IMailServerOrchestrationService, MailServerOrchestrationService>();
        services.AddTransient<IQueuedEmailOrchestrationService, QueuedEmailOrchestrationService>();
        services.AddTransient<ISentEmailOrchestrationService, SentEmailOrchestrationService>();
        services.AddTransient<IReceivedEmailOrchestrationService, ReceivedEmailOrchestrationService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.AddTransient<IMailEventHandlers, MailEventHandlers>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IMailServerEventProcessingService, MailServerEventProcessingService>();
        services.AddTransient<IMailServerProcessingService, MailServerProcessingService>();
        services.AddTransient<IMailSenderProcessingService, MailSenderProcessingService>();
        services.AddTransient<IMailReceiverProcessingService, MailReceiverProcessingService>();
        services.AddTransient<IQueuedEmailEventProcessingService, QueuedEmailEventProcessingService>();
        services.AddTransient<IQueuedEmailProcessingService, QueuedEmailProcessingService>();
        services.AddTransient<ISentEmailEventProcessingService, SentEmailEventProcessingService>();
        services.AddTransient<ISentEmailProcessingService, SentEmailProcessingService>();
        services.AddTransient<IReceivedEmailProcessingService, ReceivedEmailProcessingService>();
        services.AddTransient<IMailSendingProcessingService, MailSendingProcessingService>();
        services.AddTransient<IMailReceivingProcessingService, MailReceivingProcessingService>();
    }
}