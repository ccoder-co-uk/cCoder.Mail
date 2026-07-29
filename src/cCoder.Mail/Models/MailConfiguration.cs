// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Models;

public class MailConfiguration
{
    public MailConfiguration()
    {
        ConnectionString = string.Empty;

        RootPath = "Api/Mail";
        EventProviders = [];
        Providers = [];

    }

    public string ConnectionString { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public bool IsMigrating { get; set; }
    public EventProvider[] EventProviders { get; internal set; }
    public MailProviderRegistration[] Providers { get; set; }
}