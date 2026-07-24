// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Mail.Models;

namespace cCoder.Mail;

public static class MailConfigurationExtensions
{
    public static MailConfiguration AddSmtpSender(
        this MailConfiguration newMailConfiguration,
        string name = MailProviderNames.Smtp) =>
        newMailConfiguration.AddSenderProvider(
            name: name,
            providerName: MailProviderNames.Smtp);

    public static MailConfiguration AddPop3Receiver(
        this MailConfiguration newMailConfiguration,
        string name = MailProviderNames.Pop3) =>
        newMailConfiguration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.Pop3);

    public static MailConfiguration AddImapReceiver(
        this MailConfiguration newMailConfiguration,
        string name = MailProviderNames.Imap) =>
        newMailConfiguration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.Imap);

    public static MailConfiguration AddMicrosoftGraphSender(
        this MailConfiguration newMailConfiguration,
        Action<MicrosoftGraphMailConfiguration> newMicrosoftGraphMailConfiguration = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        newMicrosoftGraphMailConfiguration?.Invoke(obj: newMailConfiguration.MicrosoftGraph);

        newMailConfiguration.AddSenderProvider(
            name: name,
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddSenderProvider(
            name: "graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddSenderProvider(
            name: "https://graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddSenderProvider(
            name: "microsoft-graph",
            providerName: MailProviderNames.MicrosoftGraph);

        return newMailConfiguration;
    }

    public static MailConfiguration AddMicrosoftGraphReceiver(
        this MailConfiguration newMailConfiguration,
        Action<MicrosoftGraphMailConfiguration> newMicrosoftGraphMailConfiguration = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        newMicrosoftGraphMailConfiguration?.Invoke(obj: newMailConfiguration.MicrosoftGraph);

        newMailConfiguration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddReceiverProvider(
            name: "graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddReceiverProvider(
            name: "https://graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        newMailConfiguration.AddReceiverProvider(
            name: "microsoft-graph",
            providerName: MailProviderNames.MicrosoftGraph);

        return newMailConfiguration;
    }

    public static MailConfiguration AddSenderProvider(
        this MailConfiguration newMailConfiguration,
        string name,
        string providerName)
    {
        newMailConfiguration.SenderProviders[name] = providerName;

        return newMailConfiguration;
    }

    public static MailConfiguration AddReceiverProvider(
        this MailConfiguration newMailConfiguration,
        string name,
        string providerName)
    {
        newMailConfiguration.ReceiverProviders[name] = providerName;

        return newMailConfiguration;
    }

    public static string ResolveSenderProviderName(
        this MailConfiguration configuration,
        string providerName)
    {
        string name = string.IsNullOrWhiteSpace(value: providerName)
            ? configuration.DefaultSenderProviderName
            : providerName;

        if (configuration.SenderProviders.TryGetValue(
            key: name,
            value: out string configuredName))
        {
            return configuredName;
        }

        if (string.IsNullOrWhiteSpace(value: providerName)
            || name.Contains(value: '.', comparisonType: StringComparison.Ordinal)
            || name.StartsWith(
                value: "http",
                comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return configuration.DefaultSenderProviderName;
        }

        return name;
    }

    public static string ResolveReceiverProviderName(
        this MailConfiguration configuration,
        string providerName)
    {
        string name = string.IsNullOrWhiteSpace(value: providerName)
            ? configuration.DefaultReceiverProviderName
            : providerName;

        return configuration.ReceiverProviders.TryGetValue(
            key: name,
            value: out string configuredName)
                ? configuredName
                : name;
    }

    public static MailConfiguration WithEventProviders(
        this MailConfiguration configuration,
        params EventProvider[] eventProviders)
    {
        configuration.EventProviders = eventProviders ?? [];

        return configuration;
    }
}