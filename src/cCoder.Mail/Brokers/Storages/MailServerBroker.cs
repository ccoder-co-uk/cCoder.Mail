// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Mail.Brokers.Storages;

public interface IMailServerBroker
{
    IQueryable<MailServer> GetAllMailServers(bool ignoreFilters);
    ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer);
    ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer);
    ValueTask<int> DeleteMailServerAsync(MailServer deletedMailServer);
    ValueTask DeleteAllMailServersAsync(IEnumerable<MailServer> deletedMailServer);
    ValueTask DeleteAllMailServersByAppIdAsync(int appId);
    int? GetAppId(MailServer entity);
}

internal sealed class MailServerBroker(ICoreContextFactory coreContextFactory) : IMailServerBroker
{

    public IQueryable<MailServer> GetAllMailServers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.MailServers.IgnoreQueryFilters()
            : coreDataContext.MailServers;
    }

    public async ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        MailServer result = (await coreDataContext.MailServers.AddAsync(entity: newMailServer)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        MailServer result = coreDataContext.MailServers.Update(entity: updatedMailServer)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteMailServerAsync(MailServer deletedMailServer)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailServers.Remove(entity: deletedMailServer);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailServersAsync(IEnumerable<MailServer> deletedMailServer)
    {
        if (deletedMailServer == null || !deletedMailServer.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.MailServers.RemoveRange(entities: deletedMailServer);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllMailServersByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.MailServers
            .IgnoreQueryFilters()
            .Where(predicate: server => server.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(MailServer entity)
    {
        return entity.AppId;
    }
}