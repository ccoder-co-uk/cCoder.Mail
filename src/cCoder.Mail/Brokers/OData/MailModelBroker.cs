// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Mail.Models.OData;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using System.Linq.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Mail.Brokers.OData;

internal sealed class MailModelBroker
    : IMailModelBroker
{
    private readonly ODataConventionModelBuilder builder;

    public MailModelBroker(ODataConventionModelBuilder builder = null)
    {
        this.builder = builder ?? new ODataConventionModelBuilder();
    }

    public ODataModel Build()
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

    private EntitySetConfiguration<T> AddSet<T, TKey>(
        bool enableBatchingToo = false,
        string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;

        return builder.EntitySet<T>(name: setName);
    }

    private EntitySetConfiguration<T> AddJoinSet<T, TKey>(
        Expression<Func<T, TKey>> key)
        where T : class
    {
        string name = typeof(T).Name;
        EntitySetConfiguration<T> result = builder.EntitySet<T>(name: name);

        builder.EntityType<T>()
            .HasKey(keyDefinitionExpression: key);

        return result;
    }

    private void AddCommonComplextypes()
    {
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();
    }
}