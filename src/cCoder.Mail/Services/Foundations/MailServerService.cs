using System.Security;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

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
        MailServer result = await mailServerBroker.AddMailServerAsync(Copy(mailServer));
        mailServer.Id = result.Id;
        mailServer.AppId = result.AppId;
        mailServer.Name = result.Name;
        mailServer.User = result.User;
        mailServer.Password = result.Password;
        mailServer.Host = result.Host;
        mailServer.FromEmail = result.FromEmail;
        mailServer.Port = result.Port;
        mailServer.EnableSSL = result.EnableSSL;
        return mailServer;
    }

    public async ValueTask<MailServer> UpdateAsync(MailServer mailServer)
    {
        authorizationBroker.Authorize(mailServer.AppId, $"{nameof(MailServer)}_update");
        MailServer result = await mailServerBroker.UpdateMailServerAsync(Copy(mailServer));
        mailServer.Id = result.Id;
        mailServer.AppId = result.AppId;
        mailServer.Name = result.Name;
        mailServer.User = result.User;
        mailServer.Password = result.Password;
        mailServer.Host = result.Host;
        mailServer.FromEmail = result.FromEmail;
        mailServer.Port = result.Port;
        mailServer.EnableSSL = result.EnableSSL;
        return mailServer;
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


