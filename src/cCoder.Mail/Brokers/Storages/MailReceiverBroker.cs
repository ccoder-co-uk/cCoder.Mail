// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Dependencies;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IMailReceiverBroker
{
    IQueryable<MailReceiver> GetAllMailReceivers(bool ignoreFilters);
    MailReceiver[] GetEnabledMailReceivers();
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteMailReceiverAsync(MailReceiver deletedMailReceiver);
    ValueTask DeleteAllMailReceiversAsync(IEnumerable<MailReceiver> deletedMailReceiver);
    ValueTask DeleteAllMailReceiversByAppIdAsync(int appId);
    int? GetAppId(MailReceiver entity);
}

internal sealed class MailReceiverBroker(ICoreContextFactory coreContextFactory) : IMailReceiverBroker
{
    public IQueryable<MailReceiver> GetAllMailReceivers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return StorageBrokerDependency.SelectAll(
            entities: coreDataContext.MailReceivers,
            ignoreFilters: ignoreFilters);
    }

    public MailReceiver[] GetEnabledMailReceivers()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.MailReceivers
            .IgnoreQueryFilters()
            .Where(predicate: receiver => receiver.IsEnabled)
            .ToArray();
    }

    public async ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailReceiver result = (await coreDataContext.MailReceivers.AddAsync(entity: newMailReceiver)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        MailReceiver result = coreDataContext.MailReceivers.Update(entity: updatedMailReceiver)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailReceiverAsync(MailReceiver deletedMailReceiver)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailReceivers.Remove(entity: deletedMailReceiver);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailReceiversAsync(IEnumerable<MailReceiver> deletedMailReceiver)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        MailReceiver[] entities = StorageBrokerDependency.Normalize(
            entities: deletedMailReceiver);

        coreDataContext.MailReceivers.RemoveRange(entities: entities);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailReceiversByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.MailReceivers
            .IgnoreQueryFilters()
            .Where(predicate: receiver => receiver.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(MailReceiver entity) =>
        entity.AppId;
}