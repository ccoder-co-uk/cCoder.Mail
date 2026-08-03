// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Dependencies.MailClients;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Mail.Providers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMailProviders(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        MailProviderConfigurations registrations =
            providers
            ?? [];

        services.AddSingleton(implementationInstance: registrations);

        foreach (KeyValuePair<string, MailProviderConfiguration> registration
            in registrations)
        {
            services.AddKeyedSingleton(
                serviceKey: registration.Key,
                implementationInstance: registration.Value);
        }

        services.AddDependencies(providers: registrations);
        services.AddBrokers(providers: registrations);
        services.AddFoundations(providers: registrations);
        services.AddExposures(providers: registrations);
        services.AddProviderConfigurations(providers: registrations);

        return services;
    }

    private static void AddDependencies(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        if (providers.ContainsKey(key: MailProviderNames.Smtp))
        {
            services.AddTransient<SmtpMailClientDependency>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Pop3))
        {
            services.AddTransient<Pop3MailClientDependency>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Imap))
        {
            services.AddTransient<ImapMailClientDependency>();
        }

        if (providers.ContainsKey(key: MailProviderNames.MicrosoftGraph))
        {
            services.AddTransient<MicrosoftGraphMailClientDependency>();
        }
    }

    private static void AddBrokers(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        if (providers.ContainsKey(key: MailProviderNames.Smtp))
        {
            services.AddTransient<
                ISmtpMailSenderBroker,
                SmtpMailSenderBroker>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Pop3))
        {
            services.AddTransient<
                IPop3MailReceiverBroker,
                Pop3MailReceiverBroker>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Imap))
        {
            services.AddTransient<
                IImapMailReceiverBroker,
                ImapMailReceiverBroker>();
        }

        if (providers.ContainsKey(key: MailProviderNames.MicrosoftGraph))
        {
            services.AddTransient<
                IMicrosoftGraphBroker,
                MicrosoftGraphBroker>();
        }

        services.AddTransient<
            IMailReceiverStorageBroker,
            MailReceiverStorageBroker>();
    }

    private static void AddFoundations(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        if (providers.ContainsKey(key: MailProviderNames.Smtp))
        {
            services.AddTransient<
                ISmtpMailSenderService,
                SmtpMailSenderService>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Pop3))
        {
            services.AddTransient<
                IPop3MailReceiverService,
                Pop3MailReceiverService>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Imap))
        {
            services.AddTransient<
                IImapMailReceiverService,
                ImapMailReceiverService>();
        }

        if (providers.ContainsKey(key: MailProviderNames.MicrosoftGraph))
        {
            services.AddTransient<
                IMicrosoftGraphMailSenderService,
                MicrosoftGraphMailSenderService>();
            services.AddTransient<
                IMicrosoftGraphMailReceiverService,
                MicrosoftGraphMailReceiverService>();
        }

        services.AddTransient<
            IMailProviderService,
            MailProviderService>();
    }

    private static void AddExposures(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        services.AddTransient<IMailClientFactory, MailClientFactory>();

        if (providers.ContainsKey(key: MailProviderNames.Smtp))
        {
            services.AddTransient<IMailClient, SmtpMailClient>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Pop3))
        {
            services.AddTransient<IMailClient, PopMailClient>();
        }

        if (providers.ContainsKey(key: MailProviderNames.Imap))
        {
            services.AddTransient<IMailClient, ImapMailClient>();
        }

        if (providers.ContainsKey(key: MailProviderNames.MicrosoftGraph))
        {
            services.AddTransient<IMailClient, MicrosoftGraphMailClient>();
        }
    }

    private static void AddProviderConfigurations(
        this IServiceCollection services,
        MailProviderConfigurations providers)
    {
        MailProviderConfiguration graphConfiguration =
            providers.GetValueOrDefault(
                key: MailProviderNames.MicrosoftGraph);

        if (graphConfiguration is not null)
        {
            services.AddSingleton(
                implementationInstance: graphConfiguration);
            services.AddKeyedSingleton(
                serviceKey: MailProviderNames.MicrosoftGraph,
                implementationInstance: graphConfiguration);
        }
    }
}