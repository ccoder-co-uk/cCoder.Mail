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
    ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail);
    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail);
    ValueTask<int> DeleteSentEmailAsync(SentEmail deletedSentEmail);
    ValueTask DeleteAllSentEmailsAsync(IEnumerable<SentEmail> deletedSentEmail);
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

    public async ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        SentEmail result = (await coreDataContext.SentMail.AddAsync(entity: newSentEmail)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        SentEmail result = coreDataContext.SentMail.Update(entity: updatedSentEmail)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteSentEmailAsync(SentEmail deletedSentEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SentMail.Remove(entity: deletedSentEmail);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllSentEmailsAsync(IEnumerable<SentEmail> deletedSentEmail)
    {
        if (deletedSentEmail == null || !deletedSentEmail.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SentMail.RemoveRange(entities: deletedSentEmail);
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