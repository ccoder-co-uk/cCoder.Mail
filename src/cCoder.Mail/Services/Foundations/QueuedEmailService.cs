// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal partial class QueuedEmailService(
    IQueuedEmailBroker queuedEmailBroker,
    IAuthorizationBroker authorizationBroker
) : IQueuedEmailService
{
    public QueuedEmail GetQueuedEmail(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {

        ValidateGet(inputs: [queuedEmailId]);

        QueuedEmail queuedEmail = GetAllQueuedEmail()
            .FirstOrDefault(predicate: i => i.Id == queuedEmailId);

        if (queuedEmail is not null)
        {
            return queuedEmail;
        }

        QueuedEmail unrestrictedQueuedEmail = GetAllQueuedEmail(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == queuedEmailId);

        if (unrestrictedQueuedEmail is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    });

    public IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return queuedEmailBroker.GetAllQueuedEmails(ignoreFilters: ignoreFilters);
        });

    public QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures) =>
        TryCatch<QueuedEmail[]>(operation: () =>
        {
            ValidateGetDispatchBatch(inputs: [batchSize, maxFailures]);

            return queuedEmailBroker.GetDispatchBatch(batchSize: batchSize, maxFailures: maxFailures);
        });

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivileges = true) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {

        ValidateAddAsync(inputs: [newQueuedEmail, checkPrivileges]);

        if (checkPrivileges)
        {
            authorizationBroker.Authorize(appId: newQueuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_create");
        }

        QueuedEmail result = await queuedEmailBroker.AddQueuedEmailAsync(newQueuedEmail: Copy(queuedEmail: newQueuedEmail));
        newQueuedEmail.Id = result.Id;
        newQueuedEmail.AppId = result.AppId;
        newQueuedEmail.SentByUserId = result.SentByUserId;
        newQueuedEmail.Subject = result.Subject;
        newQueuedEmail.Content = result.Content;
        newQueuedEmail.To = result.To;
        newQueuedEmail.CC = result.CC;
        newQueuedEmail.IsBodyHtml = result.IsBodyHtml;
        newQueuedEmail.MailServerName = result.MailServerName;
        newQueuedEmail.MailSenderId = result.MailSenderId;
        newQueuedEmail.MailSender = result.MailSender;
        return newQueuedEmail;
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedQueuedEmail]);

        authorizationBroker.Authorize(appId: updatedQueuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_update");
        QueuedEmail result = await queuedEmailBroker.UpdateQueuedEmailAsync(updatedQueuedEmail: Copy(queuedEmail: updatedQueuedEmail));
        updatedQueuedEmail.Id = result.Id;
        updatedQueuedEmail.AppId = result.AppId;
        updatedQueuedEmail.SentByUserId = result.SentByUserId;
        updatedQueuedEmail.Subject = result.Subject;
        updatedQueuedEmail.Content = result.Content;
        updatedQueuedEmail.To = result.To;
        updatedQueuedEmail.CC = result.CC;
        updatedQueuedEmail.IsBodyHtml = result.IsBodyHtml;
        updatedQueuedEmail.MailServerName = result.MailServerName;
        updatedQueuedEmail.MailSenderId = result.MailSenderId;
        updatedQueuedEmail.MailSender = result.MailSender;
        return updatedQueuedEmail;
    }, isValueTask: true);

    public ValueTask RecordSendFailureAsync(
        int emailId,
        string reason,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateRecordSendFailureAsync(inputs: [emailId, reason, cancellationToken]);

            return queuedEmailBroker.AddQueuedEmailSendFailureAsync(emailId: emailId, reason: reason, cancellationToken: cancellationToken);
        }, isValueTask: true);

    public ValueTask MarkAsSentQueuedEmailAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateMarkAsSentAsync(inputs: [queuedEmail, mailSenderId, fromAddress, cancellationToken]);

            return queuedEmailBroker.MarkQueuedEmailAsSentAsync(entity: Copy(queuedEmail: queuedEmail), mailSenderId: mailSenderId, fromAddress: fromAddress, cancellationToken: cancellationToken);
        }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId, bool checkPrivileges = true) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId, checkPrivileges]);

        QueuedEmail queuedEmail = GetAllQueuedEmail(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == queuedEmailId);

        if (queuedEmail is null)
        {
            return;
        }

        if (checkPrivileges)
        {
            authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: $"{nameof(QueuedEmail)}_delete");
        }

        await queuedEmailBroker.DeleteAllQueuedEmailSendFailuresAsync(
deletedEmailSendFailure: queuedEmail.FailedSends?.Select(selector: Copy)
            .ToArray() ?? []);

        _ = await queuedEmailBroker.DeleteQueuedEmailAsync(deletedQueuedEmail: Copy(queuedEmail: queuedEmail));
    }, isValueTask: true);

    public ValueTask DeleteAllForAppQueuedEmailAsync(IEnumerable<QueuedEmail> deletedQueuedEmail) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllForAppAsync(inputs: [deletedQueuedEmail]);

        foreach (QueuedEmail item in deletedQueuedEmail ?? [])
        {
            await DeleteAsync(queuedEmailId: item.Id, checkPrivileges: false);
        }
    }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllByAppIdAsync(inputs: [appId]);

            return queuedEmailBroker.DeleteAllQueuedEmailsByAppIdAsync(appId: appId);
        }, isValueTask: true);

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