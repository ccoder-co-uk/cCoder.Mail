// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataQueuedEmail = cCoder.Data.Models.Mail.QueuedEmail;


namespace cCoder.Mail.Services.Foundations.Events;

internal partial class QueuedEmailEventService(
    IQueuedEmailEventBroker queuedEmailEventBroker,
    ICoreAuthInfo authInfo
) : IQueuedEmailEventService
{
    public ValueTask RaiseQueuedEmailAddEventAsync(QueuedEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseQueuedEmailAddEventAsync(inputs: [entity]);

        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity: entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailAddEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseQueuedEmailUpdateEventAsync(QueuedEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseQueuedEmailUpdateEventAsync(inputs: [entity]);

        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity: entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailUpdateEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseQueuedEmailDeleteEventAsync(QueuedEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseQueuedEmailDeleteEventAsync(inputs: [entity]);

        EventMessage<DataQueuedEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalQueuedEmail(entity: entity),
        };

        await queuedEmailEventBroker.RaiseQueuedEmailDeleteEventAsync(message: message);
    }, isValueTask: true);

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