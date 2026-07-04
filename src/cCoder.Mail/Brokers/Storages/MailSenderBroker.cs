using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IMailSenderBroker
{
    IQueryable<MailSender> GetAllMailSenders(bool ignoreFilters);
    ValueTask<MailSender> AddMailSenderAsync(MailSender entity);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender entity);
    ValueTask<int> DeleteMailSenderAsync(MailSender entity);
    ValueTask DeleteAllMailSendersAsync(IEnumerable<MailSender> items);
    ValueTask DeleteAllMailSendersByAppIdAsync(int appId);
    int? GetAppId(MailSender entity);
}

public class MailSenderBroker(ICoreContextFactory coreContextFactory) : IMailSenderBroker
{
    public IQueryable<MailSender> GetAllMailSenders(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.MailSenders.IgnoreQueryFilters()
            : coreDataContext.MailSenders;
    }

    public async ValueTask<MailSender> AddMailSenderAsync(MailSender entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailSender result = (await coreDataContext.MailSenders.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailSender> UpdateMailSenderAsync(MailSender entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailSender result = coreDataContext.MailSenders.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailSenderAsync(MailSender entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailSenders.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailSendersAsync(IEnumerable<MailSender> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailSenders.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailSendersByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.MailSenders
            .IgnoreQueryFilters()
            .Where(sender => sender.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(MailSender entity) => entity.AppId;
}
