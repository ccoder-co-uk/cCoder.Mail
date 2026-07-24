// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.Mail.Dependencies.OData;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Mail.Dependencies.OData;

public abstract class ODataModelBuilder
{
    protected ODataConventionModelBuilder Builder { get; }

    protected ODataModelBuilder(ODataConventionModelBuilder builder = null)
    {
        Builder = builder ?? new ODataConventionModelBuilder();
    }

    public abstract ODataModel Build();

    protected virtual EntitySetConfiguration<T> AddSet<T, TKey>(bool enableBatchingToo = false, string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;
        return Builder.EntitySet<T>(name: setName);
    }

    protected virtual EntitySetConfiguration<T> AddJoinSet<T, TKey>(Expression<Func<T, TKey>> key)
        where T : class
    {
        string name = typeof(T).Name;
        EntitySetConfiguration<T> result = Builder.EntitySet<T>(name: name);

        Builder.EntityType<T>()
            .HasKey(keyDefinitionExpression: key);

        return result;
    }

    protected virtual void AddCommonComplextypes()
    {
        Builder.ComplexType<MetadataContainerSet>();
        Builder.ComplexType<MetadataContainer>();
        Builder.ComplexType<PropertyContainer>();
        Builder.ComplexType<AuditResultsByUser>();
        Builder.ComplexType<AuditResultByProperty>();
    }
}