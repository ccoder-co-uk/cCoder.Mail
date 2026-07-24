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
    public MailReceiver Get(Guid id) =>
        TryCatch<MailReceiver>(operation: () =>
    {

        ValidateGet(inputs: [id]);

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
    });

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
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

    public ValueTask<MailReceiver> AddAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_create");
        return await mailReceiverBroker.AddMailReceiverAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_update");
        return await mailReceiverBroker.UpdateMailReceiverAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid id) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        MailReceiver entity = GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailReceiver)}_delete");
        return await mailReceiverBroker.DeleteMailReceiverAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items) =>
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