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
    public MailServer Get(int mailServerId) =>
        TryCatch<MailServer>(operation: () =>
    {

        ValidateGet(inputs: [mailServerId]);

        MailServer mailServer = GetAll()
            .FirstOrDefault(predicate: i => i.Id == mailServerId);

        if (mailServer is not null)
        {
            return mailServer;
        }

        MailServer unrestrictedMailServer = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == mailServerId);

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

    public ValueTask<MailServer> AddAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newMailServer]);

        authorizationBroker.Authorize(appId: newMailServer.AppId, privilege: $"{nameof(MailServer)}_create");
        MailServer result = await mailServerBroker.AddMailServerAsync(entity: Copy(mailServer: newMailServer));
        newMailServer.Id = result.Id;
        newMailServer.AppId = result.AppId;
        newMailServer.Name = result.Name;
        newMailServer.User = result.User;
        newMailServer.Password = result.Password;
        newMailServer.Host = result.Host;
        newMailServer.FromEmail = result.FromEmail;
        newMailServer.Port = result.Port;
        newMailServer.EnableSSL = result.EnableSSL;
        return newMailServer;
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailServer]);

        authorizationBroker.Authorize(appId: updatedMailServer.AppId, privilege: $"{nameof(MailServer)}_update");
        MailServer result = await mailServerBroker.UpdateMailServerAsync(entity: Copy(mailServer: updatedMailServer));
        updatedMailServer.Id = result.Id;
        updatedMailServer.AppId = result.AppId;
        updatedMailServer.Name = result.Name;
        updatedMailServer.User = result.User;
        updatedMailServer.Password = result.Password;
        updatedMailServer.Host = result.Host;
        updatedMailServer.FromEmail = result.FromEmail;
        updatedMailServer.Port = result.Port;
        updatedMailServer.EnableSSL = result.EnableSSL;
        return updatedMailServer;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int mailServerId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailServerId]);

        MailServer mailServer = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailServerId);

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