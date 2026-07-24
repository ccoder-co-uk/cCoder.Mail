// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Dependencies;
using Microsoft.EntityFrameworkCore;
using DataEmailSendFailure = cCoder.Data.Models.Mail.EmailSendFailure;


namespace cCoder.Mail.Brokers.Storages;

public interface IQueuedEmailBroker
{
    IQueryable<QueuedEmail> GetAllQueuedEmails(bool ignoreFilters);
    QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures);
    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail);
    ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail);
    ValueTask<int> DeleteQueuedEmailAsync(QueuedEmail deletedQueuedEmail);
    ValueTask AddQueuedEmailSendFailureAsync(int emailId, string reason, CancellationToken cancellationToken = default);
    ValueTask MarkQueuedEmailAsSentAsync(
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default);
    ValueTask DeleteAllQueuedEmailSendFailuresAsync(IEnumerable<DataEmailSendFailure> deletedEmailSendFailure);
    ValueTask DeleteAllQueuedEmailsAsync(IEnumerable<QueuedEmail> deletedQueuedEmail);
    ValueTask DeleteAllQueuedEmailsByAppIdAsync(int appId);
    int? GetAppId(QueuedEmail entity);
}

internal sealed class QueuedEmailBroker(ICoreContextFactory coreContextFactory) : IQueuedEmailBroker
{

    public IQueryable<QueuedEmail> GetAllQueuedEmails(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return StorageBrokerDependency.SelectAll(
            entities: coreDataContext.QueuedMail,
            ignoreFilters: ignoreFilters);
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

    public async ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        QueuedEmail result = (await coreDataContext.QueuedMail.AddAsync(entity: newQueuedEmail)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        QueuedEmail result = coreDataContext.QueuedMail.Update(entity: updatedQueuedEmail)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteQueuedEmailAsync(QueuedEmail deletedQueuedEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        EmailSendFailure[] failures = coreDataContext.SendFailures
            .Where(predicate: failure => failure.EmailId == deletedQueuedEmail.Id)
            .ToArray();

        coreDataContext.SendFailures.RemoveRange(entities: failures);

        coreDataContext.QueuedMail.Remove(entity: deletedQueuedEmail);
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

    public ValueTask MarkQueuedEmailAsSentAsync(
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default) =>
        QueuedEmailStorageDependency.MarkQueuedEmailAsSentAsync(
            coreContextFactory: coreContextFactory,
            entity: entity,
            mailSenderId: mailSenderId,
            fromAddress: fromAddress,
            cancellationToken: cancellationToken);

    public async ValueTask DeleteAllQueuedEmailSendFailuresAsync(IEnumerable<DataEmailSendFailure> deletedEmailSendFailure)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        DataEmailSendFailure[] entities = StorageBrokerDependency.Normalize(
            entities: deletedEmailSendFailure);

        coreDataContext.SendFailures.RemoveRange(entities: entities);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllQueuedEmailsAsync(IEnumerable<QueuedEmail> deletedQueuedEmail)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        QueuedEmail[] entities = StorageBrokerDependency.Normalize(
            entities: deletedQueuedEmail);

        coreDataContext.QueuedMail.RemoveRange(entities: entities);
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