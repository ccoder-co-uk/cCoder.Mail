using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Brokers.Storages;

public interface IReceivedEmailBroker
{
    IQueryable<ReceivedEmail> GetAllReceivedEmails(bool ignoreFilters);
    ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail entity);
    ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail entity);
    ValueTask<int> DeleteReceivedEmailAsync(ReceivedEmail entity);
    ValueTask AddReceivedEmailsAsync(IEnumerable<ReceivedEmail> entities, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllReceivedEmailsAsync(IEnumerable<ReceivedEmail> items);
    int? GetAppId(ReceivedEmail entity);
}

public class ReceivedEmailBroker(ICoreContextFactory coreContextFactory) : IReceivedEmailBroker
{
    public IQueryable<ReceivedEmail> GetAllReceivedEmails(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.ReceivedMail.IgnoreQueryFilters()
            : coreDataContext.ReceivedMail;
    }

    public async ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ReceivedEmail result = (await coreDataContext.ReceivedMail.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ReceivedEmail result = coreDataContext.ReceivedMail.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteReceivedEmailAsync(ReceivedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ReceivedMail.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask AddReceivedEmailsAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default)
    {
        ReceivedEmail[] items = entities?.ToArray() ?? [];

        if (items.Length == 0)
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        await coreDataContext.ReceivedMail.AddRangeAsync(items, cancellationToken);
        _ = await coreDataContext.SaveChangesAsync(cancellationToken);
    }

    public bool Exists(Guid mailReceiverId, string messageId)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            return false;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.ReceivedMail
            .IgnoreQueryFilters()
            .Any(email => email.MailReceiverId == mailReceiverId && email.MessageId == messageId);
    }

    public async ValueTask DeleteAllReceivedEmailsAsync(IEnumerable<ReceivedEmail> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ReceivedMail.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(ReceivedEmail entity) => entity.AppId;
}
