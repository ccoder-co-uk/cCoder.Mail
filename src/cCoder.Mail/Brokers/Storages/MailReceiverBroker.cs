using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IMailReceiverBroker
{
    IQueryable<MailReceiver> GetAllMailReceivers(bool ignoreFilters);
    MailReceiver[] GetEnabledMailReceivers();
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver entity);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver entity);
    ValueTask<int> DeleteMailReceiverAsync(MailReceiver entity);
    ValueTask DeleteAllMailReceiversAsync(IEnumerable<MailReceiver> items);
    ValueTask DeleteAllMailReceiversByAppIdAsync(int appId);
    int? GetAppId(MailReceiver entity);
}

public class MailReceiverBroker(ICoreContextFactory coreContextFactory) : IMailReceiverBroker
{
    public IQueryable<MailReceiver> GetAllMailReceivers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.MailReceivers.IgnoreQueryFilters()
            : coreDataContext.MailReceivers;
    }

    public MailReceiver[] GetEnabledMailReceivers()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.MailReceivers
            .IgnoreQueryFilters()
            .Where(receiver => receiver.IsEnabled)
            .ToArray();
    }

    public async ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailReceiver result = (await coreDataContext.MailReceivers.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailReceiver result = coreDataContext.MailReceivers.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailReceiverAsync(MailReceiver entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailReceivers.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailReceiversAsync(IEnumerable<MailReceiver> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailReceivers.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailReceiversByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.MailReceivers
            .IgnoreQueryFilters()
            .Where(receiver => receiver.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(MailReceiver entity) => entity.AppId;
}
