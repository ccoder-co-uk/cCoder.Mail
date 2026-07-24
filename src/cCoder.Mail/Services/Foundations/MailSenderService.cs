// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal partial class MailSenderService(
    IMailSenderBroker mailSenderBroker,
    IAuthorizationBroker authorizationBroker)
    : IMailSenderService
{
    public MailSender Get(Guid id) =>
        TryCatch<MailSender>(operation: () =>
    {

        ValidateGet(inputs: [id]);

        MailSender mailSender = GetAll()
                                               .FirstOrDefault(predicate: item => item.Id == id);

        if (mailSender is not null)
        {
            return mailSender;
        }

        MailSender unrestrictedMailSender = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (unrestrictedMailSender is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedMailSender.AppId, privilege: $"{nameof(MailSender)}_read");
        }

        return unrestrictedMailSender;
    });

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return mailSenderBroker.GetAllMailSenders(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_create");
        return await mailSenderBroker.AddMailSenderAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_update");
        return await mailSenderBroker.UpdateMailSenderAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid id) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        MailSender entity = GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_delete");
        return await mailSenderBroker.DeleteMailSenderAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<MailSender> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return mailSenderBroker.DeleteAllMailSendersAsync(items: items);
        }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllByAppIdAsync(inputs: [appId]);

            return mailSenderBroker.DeleteAllMailSendersByAppIdAsync(appId: appId);
        }, isValueTask: true);

    private static MailSender Copy(MailSender entity) =>
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
                FromEmail = entity.FromEmail,
                Port = entity.Port,
                EnableSSL = entity.EnableSSL,
            };
}