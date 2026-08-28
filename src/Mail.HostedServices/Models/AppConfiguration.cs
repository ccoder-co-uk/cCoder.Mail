// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Models;

namespace Mail.HostedServices.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        CoreData = new CoreDataConfiguration();
        Eventing = new EventingConfiguration();
        Mail = new MailConfiguration
        {
            Providers = new MailProviderConfigurations()
        };
    }

    public CoreDataConfiguration CoreData { get; set; }

    public EventingConfiguration Eventing { get; set; }

    public MailConfiguration Mail { get; set; }
}