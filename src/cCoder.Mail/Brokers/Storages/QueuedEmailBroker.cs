using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;
using DataEmailSendFailure = cCoder.Data.Models.Mail.EmailSendFailure;


namespace cCoder.Mail.Brokers.Storages;

public interface IQueuedEmailBroker
{
    IQueryable<QueuedEmail> GetAllQueuedEmails(bool ignoreFilters);
    QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures);
    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail entity);
    ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail entity);
    ValueTask<int> DeleteQueuedEmailAsync(QueuedEmail entity);
    ValueTask AddQueuedEmailSendFailureAsync(int emailId, string reason, CancellationToken cancellationToken = default);
    ValueTask MarkQueuedEmailAsSentAsync(
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default);
    ValueTask DeleteAllQueuedEmailSendFailuresAsync(IEnumerable<DataEmailSendFailure> items);
    ValueTask DeleteAllQueuedEmailsAsync(IEnumerable<QueuedEmail> items);
    ValueTask DeleteAllQueuedEmailsByAppIdAsync(int appId);
    int? GetAppId(QueuedEmail entity);
}

public class QueuedEmailBroker(ICoreContextFactory coreContextFactory) : IQueuedEmailBroker
{

    public IQueryable<QueuedEmail> GetAllQueuedEmails(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.QueuedMail.IgnoreQueryFilters()
            : coreDataContext.QueuedMail;
    }

    public QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.QueuedMail
            .IgnoreQueryFilters()
            .Include(email => email.FailedSends)
            .Include(email => email.MailSender)
            .Where(email => email.FailedSends.Count < maxFailures)
            .Take(batchSize)
            .ToArray();
    }

    public async ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        QueuedEmail result = (await coreDataContext.QueuedMail.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        QueuedEmail result = coreDataContext.QueuedMail.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        EmailSendFailure[] failures = coreDataContext.SendFailures
            .Where(failure => failure.EmailId == entity.Id)
            .ToArray();

        if (failures.Length > 0)
            coreDataContext.SendFailures.RemoveRange(failures);

        coreDataContext.QueuedMail.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask AddQueuedEmailSendFailureAsync(
        int emailId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.SendFailures.AddAsync(
            new EmailSendFailure
            {
                AttemptedOn = DateTimeOffset.UtcNow,
                EmailId = emailId,
                FailureReason = reason,
            },
            cancellationToken);

        _ = await coreDataContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask MarkQueuedEmailAsSentAsync(
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        QueuedEmail queuedEmail = await coreDataContext.QueuedMail
            .Include(email => email.FailedSends)
            .FirstOrDefaultAsync(email => email.Id == entity.Id, cancellationToken);

        if (queuedEmail == null)
            return;

        await coreDataContext.SentMail.AddAsync(
            new SentEmail
            {
                AppId = queuedEmail.AppId,
                SentByUserId = queuedEmail.SentByUserId,
                Subject = queuedEmail.Subject,
                Content = queuedEmail.Content,
                To = queuedEmail.To,
                CC = queuedEmail.CC,
                IsBodyHtml = queuedEmail.IsBodyHtml,
                SentOn = DateTimeOffset.UtcNow,
                From = fromAddress,
                MailSenderId = mailSenderId,
            },
            cancellationToken);

        if (queuedEmail.FailedSends?.Any() == true)
            coreDataContext.SendFailures.RemoveRange(queuedEmail.FailedSends);

        coreDataContext.QueuedMail.Remove(queuedEmail);
        _ = await coreDataContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DeleteAllQueuedEmailSendFailuresAsync(IEnumerable<DataEmailSendFailure> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SendFailures.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllQueuedEmailsAsync(IEnumerable<QueuedEmail> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.QueuedMail.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllQueuedEmailsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<int> queuedEmailIds =
            coreDataContext.QueuedMail
                .IgnoreQueryFilters()
                .Where(email => email.AppId == appId)
                .Select(email => email.Id);

        await coreDataContext.SendFailures
            .IgnoreQueryFilters()
            .Where(failure => queuedEmailIds.Contains(failure.EmailId))
            .ExecuteDeleteAsync();

        await coreDataContext.QueuedMail
            .IgnoreQueryFilters()
            .Where(email => email.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(QueuedEmail entity)
    {
        return entity.AppId;
    }
}







