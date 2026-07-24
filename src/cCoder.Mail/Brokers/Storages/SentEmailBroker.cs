// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Mail.Brokers.Storages;

public interface ISentEmailBroker
{
    IQueryable<SentEmail> GetAllSentEmails(bool ignoreFilters);
    ValueTask<SentEmail> AddSentEmailAsync(SentEmail entity);
    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail entity);
    ValueTask<int> DeleteSentEmailAsync(SentEmail entity);
    ValueTask DeleteAllSentEmailsAsync(IEnumerable<SentEmail> items);
    ValueTask DeleteAllSentEmailsByAppIdAsync(int appId);
    int? GetAppId(SentEmail entity);
}

internal sealed class SentEmailBroker(ICoreContextFactory coreContextFactory) : ISentEmailBroker
{

    public IQueryable<SentEmail> GetAllSentEmails(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.SentMail.IgnoreQueryFilters()
            : coreDataContext.SentMail;
    }

    public async ValueTask<SentEmail> AddSentEmailAsync(SentEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        SentEmail result = (await coreDataContext.SentMail.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        SentEmail result = coreDataContext.SentMail.Update(entity: entity)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteSentEmailAsync(SentEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SentMail.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllSentEmailsAsync(IEnumerable<SentEmail> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SentMail.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllSentEmailsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.SentMail
            .IgnoreQueryFilters()
            .Where(predicate: email => email.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(SentEmail entity)
    {
        return entity.AppId;
    }
}