// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Dependencies.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Mail.Providers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMailProviders(
        this IServiceCollection services,
        MailProviderRegistration[] providers)
    {
        MailProviderRegistration[] registrations =
            providers
            ?? [];

        services.AddSingleton(
            implementationInstance:
                registrations);

        services.AddDependencies();
        services.AddBrokers();
        services.AddFoundations();
        services.AddExposures(
            providers: registrations);
        services.AddProviderConfigurations(
            providers: registrations);

        return services;
    }

    private static void AddExposures(
        this IServiceCollection services,
        MailProviderRegistration[] providers)
    {
        services.AddTransient<
            IMailClientFactory,
            MailClientFactory>();

        if (providers.Any(
            predicate: provider =>
                string.Equals(
                    a: provider.Name,
                    b: MailProviderNames.Smtp,
                    comparisonType:
                        StringComparison.OrdinalIgnoreCase)))
        {
            services.AddTransient<IMailClient, SmtpMailClient>();
        }

        if (providers.Any(
            predicate: provider =>
                string.Equals(
                    a: provider.Name,
                    b: MailProviderNames.Pop3,
                    comparisonType:
                        StringComparison.OrdinalIgnoreCase)))
        {
            services.AddTransient<IMailClient, PopMailClient>();
        }

        if (providers.Any(
            predicate: provider =>
                string.Equals(
                    a: provider.Name,
                    b: MailProviderNames.Imap,
                    comparisonType:
                        StringComparison.OrdinalIgnoreCase)))
        {
            services.AddTransient<IMailClient, ImapMailClient>();
        }

        if (providers.Any(
            predicate: provider =>
                string.Equals(
                    a: provider.Name,
                    b: MailProviderNames.MicrosoftGraph,
                    comparisonType:
                        StringComparison.OrdinalIgnoreCase)))
        {
            services.AddTransient<
                IMailClient,
                MicrosoftGraphMailClient>();
        }
    }

    private static void AddDependencies(
        this IServiceCollection services)
    {
        services.AddTransient<SmtpMailClientDependency>();
        services.AddTransient<Pop3MailClientDependency>();
        services.AddTransient<ImapMailClientDependency>();
        services.AddTransient<MicrosoftGraphMailClientDependency>();
    }

    private static void AddBrokers(
        this IServiceCollection services)
    {
        services.AddTransient<
            ISmtpMailSenderBroker,
            SmtpMailSenderBroker>();
        services.AddTransient<
            IPop3MailReceiverBroker,
            Pop3MailReceiverBroker>();
        services.AddTransient<
            IImapMailReceiverBroker,
            ImapMailReceiverBroker>();
        services.AddTransient<
            IMicrosoftGraphBroker,
            MicrosoftGraphBroker>();
        services.AddTransient<
            IMailReceiverStorageBroker,
            MailReceiverStorageBroker>();
    }

    private static void AddFoundations(
        this IServiceCollection services)
    {
        services.AddTransient<
            ISmtpMailSenderService,
            SmtpMailSenderService>();
        services.AddTransient<
            IPop3MailReceiverService,
            Pop3MailReceiverService>();
        services.AddTransient<
            IImapMailReceiverService,
            ImapMailReceiverService>();
        services.AddTransient<
            IMicrosoftGraphMailSenderService,
            MicrosoftGraphMailSenderService>();
        services.AddTransient<
            IMicrosoftGraphMailReceiverService,
            MicrosoftGraphMailReceiverService>();
        services.AddTransient<
            IMailProviderService,
            MailProviderService>();
    }

    private static void AddProviderConfigurations(
        this IServiceCollection services,
        MailProviderRegistration[] providers)
    {
        MicrosoftGraphProviderConfiguration graphConfiguration =
            providers
                .FirstOrDefault(
                    predicate: provider =>
                        string.Equals(
                            a: provider.Name,
                            b: MailProviderNames.MicrosoftGraph,
                            comparisonType:
                                StringComparison.OrdinalIgnoreCase))
                ?.MicrosoftGraph;

        if (graphConfiguration is not null)
        {
            services.AddSingleton(
                implementationInstance:
                    graphConfiguration);
        }
    }

}