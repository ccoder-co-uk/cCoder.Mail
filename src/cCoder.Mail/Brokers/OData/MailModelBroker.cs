// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Mail.Models.OData;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Mail.Brokers.OData;

internal sealed class MailModelBroker
    : ODataModelBroker,
      IMailModelBroker
{
    public MailModelBroker(ODataConventionModelBuilder builder = null)
        : base(builder)
    {
    }

    public override ODataModel Build()
    {
        return new ODataModel
        {
            Context = "Core",
            Description = "Mail endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };
    }

    public void Configure()
    {
        ConfigureModel();
    }

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        AddSet<MailServer, int>();
        AddSet<MailSender, Guid>();
        AddSet<MailReceiver, Guid>();
        AddSet<QueuedEmail, int>();
        AddSet<SentEmail, int>();
        AddSet<ReceivedEmail, int>();
        builder.Namespace = "";
    }
}