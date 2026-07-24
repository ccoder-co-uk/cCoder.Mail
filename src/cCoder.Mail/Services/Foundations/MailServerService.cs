// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Foundations;

internal partial class MailServerService(
    IMailServerBroker mailServerBroker,
    IAuthorizationBroker authorizationBroker
) : IMailServerService
{
    public MailServer Get(int id) =>
        TryCatch<MailServer>(operation: () =>
    {

        ValidateGet(inputs: [id]);

        MailServer mailServer = GetAll()
                                               .FirstOrDefault(predicate: i => i.Id == id);

        if (mailServer is not null)
        {
            return mailServer;
        }

        MailServer unrestrictedMailServer = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == id);

        if (unrestrictedMailServer is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    });

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return mailServerBroker.GetAllMailServers(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailServer> AddAsync(MailServer mailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateAddAsync(inputs: [mailServer]);

        authorizationBroker.Authorize(appId: mailServer.AppId, privilege: $"{nameof(MailServer)}_create");
        MailServer result = await mailServerBroker.AddMailServerAsync(entity: Copy(mailServer: mailServer));
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
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer mailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [mailServer]);

        authorizationBroker.Authorize(appId: mailServer.AppId, privilege: $"{nameof(MailServer)}_update");
        MailServer result = await mailServerBroker.UpdateMailServerAsync(entity: Copy(mailServer: mailServer));
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
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        MailServer mailServer = GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        if (mailServer is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: mailServer.AppId, privilege: $"{nameof(MailServer)}_delete");
        _ = await mailServerBroker.DeleteMailServerAsync(entity: Copy(mailServer: mailServer));
    }, isValueTask: true);

    public ValueTask DeleteAllForAppAsync(IEnumerable<MailServer> items) =>
        TryCatch(operation: () =>
    {

        ValidateDeleteAllForAppAsync(inputs: [items]);

        return mailServerBroker.DeleteAllMailServersAsync(
        items: items?.Select(selector: Copy) ?? []);
    }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllByAppIdAsync(inputs: [appId]);

            return mailServerBroker.DeleteAllMailServersByAppIdAsync(appId: appId);
        }, isValueTask: true);

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