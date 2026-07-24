// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Mail.Models;

namespace cCoder.Mail;

public static class MailConfigurationExtensions
{
    public static MailConfiguration AddSmtpSender(
        this MailConfiguration configuration,
        string name = MailProviderNames.Smtp) =>
        configuration.AddSenderProvider(
            name: name,
            providerName: MailProviderNames.Smtp);

    public static MailConfiguration AddPop3Receiver(
        this MailConfiguration configuration,
        string name = MailProviderNames.Pop3) =>
        configuration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.Pop3);

    public static MailConfiguration AddImapReceiver(
        this MailConfiguration configuration,
        string name = MailProviderNames.Imap) =>
        configuration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.Imap);

    public static MailConfiguration AddMicrosoftGraphSender(
        this MailConfiguration configuration,
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(obj: configuration.MicrosoftGraph);

        configuration.AddSenderProvider(
            name: name,
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddSenderProvider(
            name: "graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddSenderProvider(
            name: "https://graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddSenderProvider(
            name: "microsoft-graph",
            providerName: MailProviderNames.MicrosoftGraph);

        return configuration;
    }

    public static MailConfiguration AddMicrosoftGraphReceiver(
        this MailConfiguration configuration,
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(obj: configuration.MicrosoftGraph);

        configuration.AddReceiverProvider(
            name: name,
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddReceiverProvider(
            name: "graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddReceiverProvider(
            name: "https://graph.microsoft.com",
            providerName: MailProviderNames.MicrosoftGraph);

        configuration.AddReceiverProvider(
            name: "microsoft-graph",
            providerName: MailProviderNames.MicrosoftGraph);

        return configuration;
    }

    public static MailConfiguration AddSenderProvider(
        this MailConfiguration configuration,
        string name,
        string providerName)
    {
        configuration.SenderProviders[name] = providerName;

        return configuration;
    }

    public static MailConfiguration AddReceiverProvider(
        this MailConfiguration configuration,
        string name,
        string providerName)
    {
        configuration.ReceiverProviders[name] = providerName;

        return configuration;
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