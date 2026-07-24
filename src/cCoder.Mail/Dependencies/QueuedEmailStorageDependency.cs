// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Dependencies;

internal static class QueuedEmailStorageDependency
{
    internal static async ValueTask MarkQueuedEmailAsSentAsync(
        ICoreContextFactory coreContextFactory,
        QueuedEmail entity,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        QueuedEmail queuedEmail = await coreDataContext.QueuedMail
            .Include(navigationPropertyPath: email => email.FailedSends)
            .FirstOrDefaultAsync(
                predicate: email => email.Id == entity.Id,
                cancellationToken: cancellationToken);

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

        EmailSendFailure[] failures =
            queuedEmail.FailedSends?.ToArray() ?? [];

        coreDataContext.SendFailures.RemoveRange(entities: failures);
        coreDataContext.QueuedMail.Remove(entity: queuedEmail);

        _ = await coreDataContext.SaveChangesAsync(
            cancellationToken: cancellationToken);
    }
}