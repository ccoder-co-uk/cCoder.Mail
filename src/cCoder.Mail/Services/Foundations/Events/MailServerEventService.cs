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
using DataMailServer = cCoder.Data.Models.Mail.MailServer;


namespace cCoder.Mail.Services.Foundations.Events;

internal partial class MailServerEventService(
    IMailServerEventBroker mailServerEventBroker,
    ICoreAuthInfo authInfo
) : IMailServerEventService
{
    public ValueTask RaiseMailServerAddEventAsync(MailServer entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseMailServerAddEventAsync(inputs: [entity]);

        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity: entity),
        };

        await mailServerEventBroker.RaiseMailServerAddEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseMailServerUpdateEventAsync(MailServer entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseMailServerUpdateEventAsync(inputs: [entity]);

        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity: entity),
        };

        await mailServerEventBroker.RaiseMailServerUpdateEventAsync(message: message);
    }, isValueTask: true);

    public ValueTask RaiseMailServerDeleteEventAsync(MailServer entity) =>
        TryCatch(operation: async () =>
    {

        ValidateRaiseMailServerDeleteEventAsync(inputs: [entity]);

        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity: entity),
        };

        await mailServerEventBroker.RaiseMailServerDeleteEventAsync(message: message);
    }, isValueTask: true);

    private static DataMailServer ToExternalMailServer(MailServer entity) =>
        entity == null ? null : new DataMailServer
        {
            Id = entity.Id,
            AppId = entity.AppId,
            Name = entity.Name,
            User = entity.User,
            Password = entity.Password,
            Host = entity.Host,
            FromEmail = entity.FromEmail,
            Port = entity.Port,
            EnableSSL = entity.EnableSSL,
        };
}