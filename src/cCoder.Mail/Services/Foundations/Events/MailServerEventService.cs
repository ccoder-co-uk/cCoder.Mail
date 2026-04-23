using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataMailServer = cCoder.Data.Models.Mail.MailServer;


namespace cCoder.Mail.Services.Foundations.Events;

internal class MailServerEventService(
    IMailServerEventBroker mailServerEventBroker,
    ICoreAuthInfo authInfo
) : IMailServerEventService
{
    public async ValueTask RaiseMailServerAddEventAsync(MailServer entity)
    {
        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity),
        };

        await mailServerEventBroker.RaiseMailServerAddEventAsync(message);
    }

    public async ValueTask RaiseMailServerUpdateEventAsync(MailServer entity)
    {
        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity),
        };

        await mailServerEventBroker.RaiseMailServerUpdateEventAsync(message);
    }

    public async ValueTask RaiseMailServerDeleteEventAsync(MailServer entity)
    {
        EventMessage<DataMailServer> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalMailServer(entity),
        };

        await mailServerEventBroker.RaiseMailServerDeleteEventAsync(message);
    }

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










