using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Mail.Brokers.Storages;

public interface IMailServerBroker
{
    IQueryable<MailServer> GetAllMailServers(bool ignoreFilters);
    ValueTask<MailServer> AddMailServerAsync(MailServer entity);
    ValueTask<MailServer> UpdateMailServerAsync(MailServer entity);
    ValueTask<int> DeleteMailServerAsync(MailServer entity);
    ValueTask DeleteAllMailServersAsync(IEnumerable<MailServer> items);
    int? GetAppId(MailServer entity);
}

public class MailServerBroker(ICoreContextFactory coreContextFactory) : IMailServerBroker
{

    public IQueryable<MailServer> GetAllMailServers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.MailServers.IgnoreQueryFilters()
            : coreDataContext.MailServers;
    }

    public async ValueTask<MailServer> AddMailServerAsync(MailServer entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailServer result = (await coreDataContext.MailServers.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailServer> UpdateMailServerAsync(MailServer entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailServer result = coreDataContext.MailServers.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailServerAsync(MailServer entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailServers.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailServersAsync(IEnumerable<MailServer> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailServers.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(MailServer entity)
    {
        return entity.AppId;
    }
}







