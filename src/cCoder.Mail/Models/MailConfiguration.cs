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
    public IDictionary<string, string> SenderProviders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDictionary<string, string> ReceiverProviders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public MicrosoftGraphMailConfiguration MicrosoftGraph { get; } = new();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; } = "Api/Mail";
    public bool IncludeLegacyCoreContext { get; set; } = true;
    public string DefaultSenderProviderName { get; set; } = MailProviderNames.Smtp;
    public string DefaultReceiverProviderName { get; set; } = MailProviderNames.MicrosoftGraph;
    public EventProvider[] EventProviders { get; private set; } = [];

    public MailConfiguration AddSmtpSender(string name = MailProviderNames.Smtp)
    {
        return AddSenderProvider(name, MailProviderNames.Smtp);
    }

    public MailConfiguration AddPop3Receiver(string name = MailProviderNames.Pop3)
    {
        return AddReceiverProvider(name, MailProviderNames.Pop3);
    }

    public MailConfiguration AddImapReceiver(string name = MailProviderNames.Imap)
    {
        return AddReceiverProvider(name, MailProviderNames.Imap);
    }

    public MailConfiguration AddMicrosoftGraphSender(
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(MicrosoftGraph);
        AddSenderProvider(name, MailProviderNames.MicrosoftGraph);
        AddSenderProvider("graph.microsoft.com", MailProviderNames.MicrosoftGraph);
        AddSenderProvider("https://graph.microsoft.com", MailProviderNames.MicrosoftGraph);
        AddSenderProvider("microsoft-graph", MailProviderNames.MicrosoftGraph);
        return this;
    }

    public MailConfiguration AddMicrosoftGraphReceiver(
        Action<MicrosoftGraphMailConfiguration> configure = null,
        string name = MailProviderNames.MicrosoftGraph)
    {
        configure?.Invoke(MicrosoftGraph);
        AddReceiverProvider(name, MailProviderNames.MicrosoftGraph);
        AddReceiverProvider("graph.microsoft.com", MailProviderNames.MicrosoftGraph);
        AddReceiverProvider("https://graph.microsoft.com", MailProviderNames.MicrosoftGraph);
        AddReceiverProvider("microsoft-graph", MailProviderNames.MicrosoftGraph);
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
        string name = string.IsNullOrWhiteSpace(providerName)
            ? DefaultSenderProviderName
            : providerName;

        if (SenderProviders.TryGetValue(name, out string configuredName))
            return configuredName;

        if (string.IsNullOrWhiteSpace(providerName)
            || name.Contains('.', StringComparison.Ordinal)
            || name.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return DefaultSenderProviderName;
        }

        return name;
    }

    public string ResolveReceiverProviderName(string providerName)
    {
        string name = string.IsNullOrWhiteSpace(providerName)
            ? DefaultReceiverProviderName
            : providerName;

        return ReceiverProviders.TryGetValue(name, out string configuredName)
            ? configuredName
            : name;
    }

    public MailConfiguration WithEventProviders(params EventProvider[] eventProviders)
    {
        EventProviders = eventProviders ?? [];
        return this;
    }
}
