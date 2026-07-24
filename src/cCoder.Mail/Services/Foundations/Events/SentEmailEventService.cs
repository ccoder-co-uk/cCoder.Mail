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
using DataSentEmail = cCoder.Data.Models.Mail.SentEmail;


namespace cCoder.Mail.Services.Foundations.Events;

internal class SentEmailEventService(
    ISentEmailEventBroker sentEmailEventBroker,
    ICoreAuthInfo authInfo
) : ISentEmailEventService
{
    public async ValueTask RaiseSentEmailAddEventAsync(SentEmail entity)
    {
        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailAddEventAsync(message: message);
    }

    public async ValueTask RaiseSentEmailUpdateEventAsync(SentEmail entity)
    {
        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseSentEmailDeleteEventAsync(SentEmail entity)
    {
        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailDeleteEventAsync(message: message);
    }

    private static DataSentEmail ToExternalSentEmail(SentEmail entity) =>
        entity == null ? null : new DataSentEmail
        {
            Id = entity.Id,
            AppId = entity.AppId,
            SentByUserId = entity.SentByUserId,
            Subject = entity.Subject,
            Content = entity.Content,
            To = entity.To,
            CC = entity.CC,
            IsBodyHtml = entity.IsBodyHtml,
            SentOn = entity.SentOn,
            From = entity.From,
        };
}