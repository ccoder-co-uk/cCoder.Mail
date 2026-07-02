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
        QueuedEmail queuedEmail = GetAll().FirstOrDefault(i => i.Id == id);
        if (queuedEmail is not null)
            return queuedEmail;

        QueuedEmail unrestrictedQueuedEmail = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedQueuedEmail is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        queuedEmailBroker.GetAllQueuedEmails(ignoreFilters);

    public QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures) =>
        queuedEmailBroker.GetDispatchBatch(batchSize, maxFailures);

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail queuedEmail, bool checkPrivileges = true)
    {
        if (checkPrivileges)
            authorizationBroker.Authorize(queuedEmail.AppId, $"{nameof(QueuedEmail)}_create");

        QueuedEmail result = await queuedEmailBroker.AddQueuedEmailAsync(Copy(queuedEmail));
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
        authorizationBroker.Authorize(queuedEmail.AppId, $"{nameof(QueuedEmail)}_update");
        QueuedEmail result = await queuedEmailBroker.UpdateQueuedEmailAsync(Copy(queuedEmail));
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
        queuedEmailBroker.AddQueuedEmailSendFailureAsync(emailId, reason, cancellationToken);

    public ValueTask MarkAsSentAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default) =>
        queuedEmailBroker.MarkQueuedEmailAsSentAsync(Copy(queuedEmail), mailSenderId, fromAddress, cancellationToken);

    public async ValueTask DeleteAsync(int id, bool checkPrivileges = true)
    {
        QueuedEmail queuedEmail = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (queuedEmail is null)
            return;

        if (checkPrivileges)
            authorizationBroker.Authorize(queuedEmail.AppId, $"{nameof(QueuedEmail)}_delete");

        await queuedEmailBroker.DeleteAllQueuedEmailSendFailuresAsync(
            queuedEmail.FailedSends?.Select(Copy).ToArray() ?? []);

        _ = await queuedEmailBroker.DeleteQueuedEmailAsync(Copy(queuedEmail));
    }

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
                FailedSends = queuedEmail.FailedSends?.Select(Copy).ToArray(),
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


