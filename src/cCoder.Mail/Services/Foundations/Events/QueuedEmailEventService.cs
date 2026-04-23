using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataQueuedEmail = cCoder.Data.Models.Mail.QueuedEmail;


namespace cCoder.Mail.Services.Foundations.Events;

internal class QueuedEmailEventService(
    IQueuedEmailEventBroker queuedEmailEventBroker,
    ICoreAuthInfo authInfo
) : IQueuedEmailEventService
{
    public async ValueTask RaiseQueuedEmailAddEventAsync(QueuedEmail entity)
    {
        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailAddEventAsync(message);
    }

    public async ValueTask RaiseQueuedEmailUpdateEventAsync(QueuedEmail entity)
    {
        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailUpdateEventAsync(message);
    }

    public async ValueTask RaiseQueuedEmailDeleteEventAsync(QueuedEmail entity)
    {
        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailDeleteEventAsync(message);
    }

    private static DataQueuedEmail ToExternalQueuedEmail(QueuedEmail entity) =>
        entity == null ? null : new DataQueuedEmail
        {
            Id = entity.Id,
            AppId = entity.AppId,
            SentByUserId = entity.SentByUserId,
            Subject = entity.Subject,
            Content = entity.Content,
            To = entity.To,
            CC = entity.CC,
            IsBodyHtml = entity.IsBodyHtml,
            MailServerName = entity.MailServerName,
        };
}










