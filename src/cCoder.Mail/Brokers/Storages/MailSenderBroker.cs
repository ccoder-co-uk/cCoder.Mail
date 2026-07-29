// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Extensions;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IMailSenderBroker
{
    IQueryable<MailSender> GetAllMailSenders();
    IQueryable<MailSender> GetAllMailSendersIgnoringFilters();
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteMailSenderAsync(MailSender deletedMailSender);
    ValueTask DeleteAllMailSendersAsync(IEnumerable<MailSender> deletedMailSender);
    ValueTask DeleteAllMailSendersByAppIdAsync(int appId);
    int? GetAppId(MailSender entity);
}

internal sealed class MailSenderBroker(ICoreContextFactory coreContextFactory) : IMailSenderBroker
{
    public IQueryable<MailSender> GetAllMailSenders()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.MailSenders;
    }

    public IQueryable<MailSender> GetAllMailSendersIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.MailSenders.IgnoreQueryFilters();
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

        MailSender[] entities = deletedMailSender?.ToArray() ?? [];

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