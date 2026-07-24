// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal class MailReceiverService(
    IMailReceiverBroker mailReceiverBroker,
    IAuthorizationBroker authorizationBroker)
    : IMailReceiverService
{
    public MailReceiver Get(Guid id)
    {
        MailReceiver mailReceiver = GetAll()
            .FirstOrDefault(predicate: item => item.Id == id);

        if (mailReceiver is not null)
        {
            return mailReceiver;
        }

        MailReceiver unrestrictedMailReceiver = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (unrestrictedMailReceiver is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_read");
        }

        return unrestrictedMailReceiver;
    }

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
        mailReceiverBroker.GetAllMailReceivers(ignoreFilters: ignoreFilters);

    public MailReceiver[] GetEnabled() =>
        mailReceiverBroker.GetEnabledMailReceivers();

    public async ValueTask<MailReceiver> AddAsync(MailReceiver entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_create");
        return await mailReceiverBroker.AddMailReceiverAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<MailReceiver> UpdateAsync(MailReceiver entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_update");
        return await mailReceiverBroker.UpdateMailReceiverAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<int> DeleteAsync(Guid id)
    {
        MailReceiver entity = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_delete");
        return await mailReceiverBroker.DeleteMailReceiverAsync(entity: Copy(entity: entity));
    }

    public ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items) =>
        mailReceiverBroker.DeleteAllMailReceiversAsync(items: items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        mailReceiverBroker.DeleteAllMailReceiversByAppIdAsync(appId: appId);

    private static MailReceiver Copy(MailReceiver entity) =>
        entity is null
            ? null
            : new()
            {
                Id = entity.Id,
                AppId = entity.AppId,
                Name = entity.Name,
                ProviderName = entity.ProviderName,
                User = entity.User,
                Password = entity.Password,
                Host = entity.Host,
                Port = entity.Port,
                EnableSSL = entity.EnableSSL,
                LastReceivedOn = entity.LastReceivedOn,
                IsEnabled = entity.IsEnabled,
            };
}