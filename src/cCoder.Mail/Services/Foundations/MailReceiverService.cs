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

        ValidateMailReceiverOnGet(inputs: [mailReceiverId]);

        MailReceiver mailReceiver = mailReceiverBroker
            .GetAllMailReceivers(ignoreFilters: false)
            .FirstOrDefault(predicate: item => item.Id == mailReceiverId);

        if (mailReceiver is not null)
        {
            return mailReceiver;
        }

        MailReceiver unrestrictedMailReceiver = mailReceiverBroker
            .GetAllMailReceivers(ignoreFilters: true)
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
            ValidateAllMailReceiverOnGet(inputs: [ignoreFilters]);

            return mailReceiverBroker.GetAllMailReceivers(ignoreFilters: ignoreFilters);
        });

    public MailReceiver[] GetEnabled() =>
        TryCatch<MailReceiver[]>(operation: () =>
        {
            ValidateEnabledOnGet(inputs: []);

            return mailReceiverBroker.GetEnabledMailReceivers();
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateMailReceiverOnAdd(inputs: [newMailReceiver]);

        authorizationBroker.Authorize(appId: newMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_create");
        return await mailReceiverBroker.AddMailReceiverAsync(newMailReceiver: Copy(entity: newMailReceiver));
    }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateMailReceiverOnUpdate(inputs: [updatedMailReceiver]);

        authorizationBroker.Authorize(appId: updatedMailReceiver.AppId, privilege: $"{nameof(MailReceiver)}_update");
        return await mailReceiverBroker.UpdateMailReceiverAsync(updatedMailReceiver: Copy(entity: updatedMailReceiver));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailReceiverId) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailReceiverId]);

        MailReceiver entity = mailReceiverBroker
            .GetAllMailReceivers(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailReceiverId);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_delete");
        return await mailReceiverBroker.DeleteMailReceiverAsync(deletedMailReceiver: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver) =>
        TryCatch(operation: () =>
        {
            ValidateAllMailReceiverOnDelete(inputs: [deletedMailReceiver]);

            return mailReceiverBroker.DeleteAllMailReceiversAsync(deletedMailReceiver: deletedMailReceiver);
        }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateAllByAppIdOnDelete(inputs: [appId]);

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