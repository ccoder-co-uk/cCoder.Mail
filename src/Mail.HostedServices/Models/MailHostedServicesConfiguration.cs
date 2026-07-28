// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Mail.Models;

namespace Mail.HostedServices.Models;

public sealed class MailHostedServicesConfiguration
{
    public MailHostedServicesConfiguration()
    {
        Mail = new MailConfiguration();
        Data = new DataConfiguration();
        Eventing = new EventingConfiguration();
    }

    public MailConfiguration Mail { get; set; }
    public DataConfiguration Data { get; set; }
    public EventingConfiguration Eventing { get; set; }
}