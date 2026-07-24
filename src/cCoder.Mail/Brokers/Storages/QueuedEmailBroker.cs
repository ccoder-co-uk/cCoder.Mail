// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

internal sealed class QueuedEmailBroker(ICoreContextFactory coreContextFactory) : IQueuedEmailBroker
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
            .Include(navigationPropertyPath: email => email.FailedSends)
            .Include(navigationPropertyPath: email => email.MailSender)
            .Where(predicate: email => email.FailedSends.Count < maxFailures)
            .Take(count: batchSize)
            .ToArray();
    }

    public async ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        QueuedEmail result = (await coreDataContext.QueuedMail.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        QueuedEmail result = coreDataContext.QueuedMail.Update(entity: entity)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteQueuedEmailAsync(QueuedEmail entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        EmailSendFailure[] failures = coreDataContext.SendFailures
            .Where(predicate: failure => failure.EmailId == entity.Id)
            .ToArray();

        if (failures.Length > 0)
        {
            coreDataContext.SendFailures.RemoveRange(entities: failures);
        }

        coreDataContext.QueuedMail.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask AddQueuedEmailSendFailureAsync(
        int emailId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.SendFailures.AddAsync(
entity: new EmailSendFailure
{
    AttemptedOn = DateTimeOffset.UtcNow,
    EmailId = emailId,
    FailureReason = reason,
},
cancellationToken: cancellationToken);

        _ = await coreDataContext.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask MarkQueuedEmailAsSentAsync(
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        QueuedEmail queuedEmail = await coreDataContext.QueuedMail
            .Include(navigationPropertyPath: email => email.FailedSends)
            .FirstOrDefaultAsync(predicate: email => email.Id == entity.Id, cancellationToken: cancellationToken);

        if (queuedEmail == null)
        {
            return;
        }

        await coreDataContext.SentMail.AddAsync(
entity: new SentEmail
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
cancellationToken: cancellationToken);

        if (queuedEmail.FailedSends?.Any() == true)
        {
            coreDataContext.SendFailures.RemoveRange(entities: queuedEmail.FailedSends);
        }

        coreDataContext.QueuedMail.Remove(entity: queuedEmail);
        _ = await coreDataContext.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DeleteAllQueuedEmailSendFailuresAsync(IEnumerable<DataEmailSendFailure> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.SendFailures.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllQueuedEmailsAsync(IEnumerable<QueuedEmail> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.QueuedMail.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllQueuedEmailsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<int> queuedEmailIds =
            coreDataContext.QueuedMail
                .IgnoreQueryFilters()
            .Where(predicate: email => email.AppId == appId)
            .Select(selector: email => email.Id);

        await coreDataContext.SendFailures
            .IgnoreQueryFilters()
            .Where(predicate: failure => queuedEmailIds.Contains(item: failure.EmailId))
            .ExecuteDeleteAsync();

        await coreDataContext.QueuedMail
            .IgnoreQueryFilters()
            .Where(predicate: email => email.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(QueuedEmail entity)
    {
        return entity.AppId;
    }
}