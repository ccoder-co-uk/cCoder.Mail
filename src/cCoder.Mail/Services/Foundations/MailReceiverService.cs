// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal partial class MailReceiverService(
    IMailReceiverBroker mailReceiverBroker,
    IAuthorizationBroker authorizationBroker)
    : IMailReceiverService
{
    public MailReceiver GetMailReceiver(Guid mailReceiverId) =>
        TryCatch<MailReceiver>(operation: () =>
    {

        ValidateGet(inputs: [mailReceiverId]);

        MailReceiver mailReceiver = GetAllMailReceiver()
            .FirstOrDefault(predicate: item => item.Id == mailReceiverId);

        if (mailReceiver is not null)
        {
            return mailReceiver;
        }

        MailReceiver unrestrictedMailReceiver = GetAllMailReceiver(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailReceiverId);

        if (unrestrictedMailReceiver is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_read");
        }

        return unrestrictedMailReceiver;
    });

    public IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return mailReceiverBroker.GetAllMailReceivers(ignoreFilters: ignoreFilters);
        });

    public MailReceiver[] GetEnabled() =>
        TryCatch<MailReceiver[]>(operation: () =>
        {
            ValidateGetEnabled(inputs: []);

            return mailReceiverBroker.GetEnabledMailReceivers();
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newMailReceiver]);

        authorizationBroker.Authorize(appId: newMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_create");
        return await mailReceiverBroker.AddMailReceiverAsync(entity: Copy(entity: newMailReceiver));
    }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailReceiver]);

        authorizationBroker.Authorize(appId: updatedMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_update");
        return await mailReceiverBroker.UpdateMailReceiverAsync(entity: Copy(entity: updatedMailReceiver));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailReceiverId) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailReceiverId]);

        MailReceiver entity = GetAllMailReceiver(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailReceiverId);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_delete");
        return await mailReceiverBroker.DeleteMailReceiverAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return mailReceiverBroker.DeleteAllMailReceiversAsync(items: items);
        }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllByAppIdAsync(inputs: [appId]);

            return mailReceiverBroker.DeleteAllMailReceiversByAppIdAsync(appId: appId);
        }, isValueTask: true);

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