// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Mail.Models;
using cCoder.Security.Models;

namespace Mail.Web.Models;

public sealed class MailWebConfiguration
{
    public MailWebConfiguration()
    {
        Mail = new MailConfiguration();
        Data = new DataConfiguration();
        Security = new SecurityConfiguration();
        Eventing = new EventingConfiguration();
    }

    public MailConfiguration Mail { get; set; }
    public DataConfiguration Data { get; set; }
    public SecurityConfiguration Security { get; set; }
    public EventingConfiguration Eventing { get; set; }
}