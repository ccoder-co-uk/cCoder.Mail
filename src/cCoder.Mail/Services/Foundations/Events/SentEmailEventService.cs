// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataSentEmail = cCoder.Data.Models.Mail.SentEmail;


namespace cCoder.Mail.Services.Foundations.Events;

internal partial class SentEmailEventService(
    ISentEmailEventBroker sentEmailEventBroker,
    IAuthInfoBroker authInfoBroker
) : ISentEmailEventService
{
    public ValueTask RaiseSentEmailAddEventAsync(SentEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseSentEmailAddEventAsync(inputs: [entity]);

        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSsoUserId() },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailAddEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseSentEmailUpdateEventAsync(SentEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseSentEmailUpdateEventAsync(inputs: [entity]);

        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSsoUserId() },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailUpdateEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseSentEmailDeleteEventAsync(SentEmail entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseSentEmailDeleteEventAsync(inputs: [entity]);

        EventMessage<DataSentEmail> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSsoUserId() },
            Data = ToExternalSentEmail(entity: entity),
        };

        await sentEmailEventBroker.RaiseSentEmailDeleteEventAsync(message: message);
    }, isValueTask: true);

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