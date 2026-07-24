// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal class QueuedEmailService(
    IQueuedEmailBroker queuedEmailBroker,
    IAuthorizationBroker authorizationBroker
) : IQueuedEmailService
{
    public QueuedEmail Get(int id)
    {
        QueuedEmail queuedEmail = GetAll()
            .FirstOrDefault(predicate: i => i.Id == id);

        if (queuedEmail is not null)
            return queuedEmail;

        QueuedEmail unrestrictedQueuedEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == id);

        if (unrestrictedQueuedEmail is not null)
            throw new SecurityException(message: "Access Denied!");

        return null;
    }

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        queuedEmailBroker.GetAllQueuedEmails(ignoreFilters: ignoreFilters);

    public QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures) =>
        queuedEmailBroker.GetDispatchBatch(batchSize: batchSize, maxFailures: maxFailures);

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail queuedEmail, bool checkPrivileges = true)
    {
        if (checkPrivileges)
            authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_create");

        QueuedEmail result = await queuedEmailBroker.AddQueuedEmailAsync(entity: Copy(queuedEmail: queuedEmail));
        queuedEmail.Id = result.Id;
        queuedEmail.AppId = result.AppId;
        queuedEmail.SentByUserId = result.SentByUserId;
        queuedEmail.Subject = result.Subject;
        queuedEmail.Content = result.Content;
        queuedEmail.To = result.To;
        queuedEmail.CC = result.CC;
        queuedEmail.IsBodyHtml = result.IsBodyHtml;
        queuedEmail.MailServerName = result.MailServerName;
        queuedEmail.MailSenderId = result.MailSenderId;
        queuedEmail.MailSender = result.MailSender;
        return queuedEmail;
    }

    public async ValueTask<QueuedEmail> UpdateAsync(QueuedEmail queuedEmail)
    {
        authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_update");
        QueuedEmail result = await queuedEmailBroker.UpdateQueuedEmailAsync(entity: Copy(queuedEmail: queuedEmail));
        queuedEmail.Id = result.Id;
        queuedEmail.AppId = result.AppId;
        queuedEmail.SentByUserId = result.SentByUserId;
        queuedEmail.Subject = result.Subject;
        queuedEmail.Content = result.Content;
        queuedEmail.To = result.To;
        queuedEmail.CC = result.CC;
        queuedEmail.IsBodyHtml = result.IsBodyHtml;
        queuedEmail.MailServerName = result.MailServerName;
        queuedEmail.MailSenderId = result.MailSenderId;
        queuedEmail.MailSender = result.MailSender;
        return queuedEmail;
    }

    public ValueTask RecordSendFailureAsync(
        int emailId,
        string reason,
        CancellationToken cancellationToken = default) =>
        queuedEmailBroker.AddQueuedEmailSendFailureAsync(emailId: emailId, reason: reason, cancellationToken: cancellationToken);

    public ValueTask MarkAsSentAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default) =>
        queuedEmailBroker.MarkQueuedEmailAsSentAsync(entity: Copy(queuedEmail: queuedEmail), mailSenderId: mailSenderId, fromAddress: fromAddress, cancellationToken: cancellationToken);

    public async ValueTask DeleteAsync(int id, bool checkPrivileges = true)
    {
        QueuedEmail queuedEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (queuedEmail is null)
            return;

        if (checkPrivileges)
            authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_delete");

        await queuedEmailBroker.DeleteAllQueuedEmailSendFailuresAsync(
items: queuedEmail.FailedSends?.Select(selector: Copy)
            .ToArray() ?? []);

        _ = await queuedEmailBroker.DeleteQueuedEmailAsync(entity: Copy(queuedEmail: queuedEmail));
    }

    public async ValueTask DeleteAllForAppAsync(IEnumerable<QueuedEmail> items)
    {
        foreach (QueuedEmail item in items ?? [])
            await DeleteAsync(id: item.Id, checkPrivileges: false);
    }

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        queuedEmailBroker.DeleteAllQueuedEmailsByAppIdAsync(appId: appId);

    private static QueuedEmail Copy(QueuedEmail queuedEmail) =>
        queuedEmail == null
            ? null
            : new QueuedEmail
            {
                Id = queuedEmail.Id,
                AppId = queuedEmail.AppId,
                SentByUserId = queuedEmail.SentByUserId,
                Subject = queuedEmail.Subject,
                Content = queuedEmail.Content,
                To = queuedEmail.To,
                CC = queuedEmail.CC,
                IsBodyHtml = queuedEmail.IsBodyHtml,
                MailServerName = queuedEmail.MailServerName,
                MailSenderId = queuedEmail.MailSenderId,
                MailSender = queuedEmail.MailSender,
                FailedSends = queuedEmail.FailedSends?.Select(selector: Copy)
        .ToArray(),
            };

    private static EmailSendFailure Copy(EmailSendFailure emailSendFailure) =>
        emailSendFailure == null
            ? null
            : new EmailSendFailure
            {
                Id = emailSendFailure.Id,
                EmailId = emailSendFailure.EmailId,
                AttemptedOn = emailSendFailure.AttemptedOn,
                FailureReason = emailSendFailure.FailureReason,
            };
}