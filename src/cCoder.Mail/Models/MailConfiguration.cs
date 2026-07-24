// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Mail.Models;

public class MailConfiguration
{
    public MailConfiguration()
    {
        AddSmtpSender();
        AddMicrosoftGraphSender();
        AddPop3Receiver();
        AddImapReceiver();
        AddMicrosoftGraphReceiver();
    }

    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> SenderProviders { get; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);
    public IDictionary<string, string> ReceiverProviders { get; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);
    public MicrosoftGraphMailConfiguration MicrosoftGraph { get; } = new();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; } = "Api/Mail";
    public bool IncludeLegacyCoreContext { get; set; } = true;
    public bool IsMigrating { get; set; }
    public string DefaultSenderProviderName { get; set; } = MailProviderNames.Smtp;
    public string DefaultReceiverProviderName { get; set; } = MailProviderNames.MicrosoftGraph;
    public MailboxReceiveConfiguration Pop3 { get; } = new() { Port = 995 };
    public MailboxReceiveConfiguration Imap { get; } = new() { Port = 993 };
    public EventProvider[] EventProviders { get; private set; } = [];

    public MailConfiguration AddSmtpSender(string name = MailProviderNames.Smtp)
    {
        return AddSenderProvider(name: name, providerName: MailProviderNames.Smtp);
    }

    public MailConfiguration AddPop3Receiver(string name = MailProviderNames.Pop3)
    {
        return AddReceiverProvider(name: name, providerName: MailProviderNames.Pop3);
    }

    public MailConfiguration AddImapReceiver(string name = MailProviderNames.Imap)
    {
        return AddReceiverProvider(name: name, providerName: MailProviderNames.Imap);
    }

    public MailConfiguration AddMicrosoftGraphSender(
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(obj: MicrosoftGraph);
        AddSenderProvider(name: name, providerName: MailProviderNames.MicrosoftGraph);
        AddSenderProvider(name: "graph.microsoft.com", providerName: MailProviderNames.MicrosoftGraph);
        AddSenderProvider(name: "https://graph.microsoft.com", providerName: MailProviderNames.MicrosoftGraph);
        AddSenderProvider(name: "microsoft-graph", providerName: MailProviderNames.MicrosoftGraph);
        return this;
    }

    public MailConfiguration AddMicrosoftGraphReceiver(
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(obj: MicrosoftGraph);
        AddReceiverProvider(name: name, providerName: MailProviderNames.MicrosoftGraph);
        AddReceiverProvider(name: "graph.microsoft.com", providerName: MailProviderNames.MicrosoftGraph);
        AddReceiverProvider(name: "https://graph.microsoft.com", providerName: MailProviderNames.MicrosoftGraph);
        AddReceiverProvider(name: "microsoft-graph", providerName: MailProviderNames.MicrosoftGraph);
        return this;
    }

    public MailConfiguration AddSenderProvider(string name, string providerName)
    {
        SenderProviders[name] = providerName;
        return this;
    }

    public MailConfiguration AddReceiverProvider(string name, string providerName)
    {
        ReceiverProviders[name] = providerName;
        return this;
    }

    public string ResolveSenderProviderName(string providerName)
    {
        string name = string.IsNullOrWhiteSpace(value: providerName)
            ? DefaultSenderProviderName
            : providerName;

        if (SenderProviders.TryGetValue(key: name, value: out string configuredName))
        {
            return configuredName;
        }

        if (string.IsNullOrWhiteSpace(value: providerName)
            || name.Contains(value: '.', comparisonType: StringComparison.Ordinal)
            || name.StartsWith(value: "http", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return DefaultSenderProviderName;
        }

        return name;
    }

    public string ResolveReceiverProviderName(string providerName)
    {
        string name = string.IsNullOrWhiteSpace(value: providerName)
            ? DefaultReceiverProviderName
            : providerName;

        return ReceiverProviders.TryGetValue(key: name, value: out string configuredName)
            ? configuredName
            : name;
    }

    public MailConfiguration WithEventProviders(params EventProvider[] eventProviders)
    {
        EventProviders = eventProviders ?? [];
        return this;
    }
}