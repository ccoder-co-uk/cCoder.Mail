// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Mail.Models;

public class MailConfiguration
{
    public MailConfiguration()
    {
        ConnectionStrings = new Dictionary<string, string>();
        Settings = new Dictionary<string, string>();
        Services = new Dictionary<string, string>();

        SenderProviders = new Dictionary<string, string>(
            comparer: StringComparer.OrdinalIgnoreCase);

        ReceiverProviders = new Dictionary<string, string>(
            comparer: StringComparer.OrdinalIgnoreCase);

        MicrosoftGraph = new MicrosoftGraphMailConfiguration();
        RootPath = "Api/Mail";
        IncludeLegacyCoreContext = true;
        DefaultSenderProviderName = MailProviderNames.Smtp;
        DefaultReceiverProviderName = MailProviderNames.MicrosoftGraph;
        Pop3 = new MailboxReceiveConfiguration { Port = 995 };
        Imap = new MailboxReceiveConfiguration { Port = 993 };
        EventProviders = [];

        MailConfigurationExtensions.AddSmtpSender(newMailConfiguration: this);
        MailConfigurationExtensions.AddMicrosoftGraphSender(newMailConfiguration: this);
        MailConfigurationExtensions.AddPop3Receiver(newMailConfiguration: this);
        MailConfigurationExtensions.AddImapReceiver(newMailConfiguration: this);
        MailConfigurationExtensions.AddMicrosoftGraphReceiver(newMailConfiguration: this);
    }

    public IDictionary<string, string> ConnectionStrings { get; set; }
    public IDictionary<string, string> Settings { get; set; }
    public IDictionary<string, string> Services { get; set; }
    public IDictionary<string, string> SenderProviders { get; }
    public IDictionary<string, string> ReceiverProviders { get; }
    public MicrosoftGraphMailConfiguration MicrosoftGraph { get; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public bool IncludeLegacyCoreContext { get; set; }
    public bool IsMigrating { get; set; }
    public string DefaultSenderProviderName { get; set; }
    public string DefaultReceiverProviderName { get; set; }
    public MailboxReceiveConfiguration Pop3 { get; }
    public MailboxReceiveConfiguration Imap { get; }
    public EventProvider[] EventProviders { get; internal set; }
}