// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Dependencies;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IMailSenderBroker
{
    IQueryable<MailSender> GetAllMailSenders(bool ignoreFilters);
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteMailSenderAsync(MailSender deletedMailSender);
    ValueTask DeleteAllMailSendersAsync(IEnumerable<MailSender> deletedMailSender);
    ValueTask DeleteAllMailSendersByAppIdAsync(int appId);
    int? GetAppId(MailSender entity);
}

internal sealed class MailSenderBroker(ICoreContextFactory coreContextFactory) : IMailSenderBroker
{
    public IQueryable<MailSender> GetAllMailSenders(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return StorageBrokerDependency.SelectAll(
            entities: coreDataContext.MailSenders,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailSender result = (await coreDataContext.MailSenders.AddAsync(entity: newMailSender)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        MailSender result = coreDataContext.MailSenders.Update(entity: updatedMailSender)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailSenderAsync(MailSender deletedMailSender)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailSenders.Remove(entity: deletedMailSender);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailSendersAsync(IEnumerable<MailSender> deletedMailSender)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        MailSender[] entities = StorageBrokerDependency.Normalize(
            entities: deletedMailSender);

        coreDataContext.MailSenders.RemoveRange(entities: entities);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailSendersByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.MailSenders
            .IgnoreQueryFilters()
            .Where(predicate: sender => sender.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(MailSender entity) =>
        entity.AppId;
}