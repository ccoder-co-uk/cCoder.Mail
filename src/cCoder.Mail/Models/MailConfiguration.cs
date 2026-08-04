// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Models;

public class MailConfiguration
{
    public string ConnectionString { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public bool IsMigrating { get; set; }
    public EventProvider[] EventProviders { get; internal set; }
    public MailProviderConfigurations Providers { get; set; }
}