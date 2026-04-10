using System.Security;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;

namespace cCoder.Mail.Services.Foundations;

internal class MailServerService(
    IMailServerBroker mailServerBroker,
    IAuthorizationBroker authorizationBroker
) : IMailServerService
{
    public MailServer Get(int id)
    {
        MailServer mailServer = GetAll().FirstOrDefault(i => i.Id == id);
        if (mailServer is not null)
            return mailServer;

        MailServer unrestrictedMailServer = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedMailServer is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        mailServerBroker.GetAllMailServers(ignoreFilters);

    public async ValueTask<MailServer> AddAsync(MailServer mailServer)
    {
        authorizationBroker.Authorize(mailServer.AppId, $"{nameof(MailServer)}_create");
        return await mailServerBroker.AddMailServerAsync(Copy(mailServer));
    }

    public async ValueTask<MailServer> UpdateAsync(MailServer mailServer)
    {
        authorizationBroker.Authorize(mailServer.AppId, $"{nameof(MailServer)}_update");
        return await mailServerBroker.UpdateMailServerAsync(Copy(mailServer));
    }

    public async ValueTask DeleteAsync(int id)
    {
        MailServer mailServer = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (mailServer is null)
            return;

        authorizationBroker.Authorize(mailServer.AppId, $"{nameof(MailServer)}_delete");
        _ = await mailServerBroker.DeleteMailServerAsync(Copy(mailServer));
    }

    private static MailServer Copy(MailServer mailServer) =>
        mailServer == null
            ? null
            : new MailServer
            {
                Id = mailServer.Id,
                AppId = mailServer.AppId,
                Name = mailServer.Name,
                User = mailServer.User,
                Password = mailServer.Password,
                Host = mailServer.Host,
                FromEmail = mailServer.FromEmail,
                Port = mailServer.Port,
                EnableSSL = mailServer.EnableSSL,
            };
}


