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
    public MailSender GetMailSender(Guid mailSenderId) =>
        TryCatch<MailSender>(operation: () =>
    {

        ValidateGet(inputs: [mailSenderId]);

        MailSender mailSender = GetAllMailSender()
            .FirstOrDefault(predicate: item => item.Id == mailSenderId);

        if (mailSender is not null)
        {
            return mailSender;
        }

        MailSender unrestrictedMailSender = GetAllMailSender(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailSenderId);

        if (unrestrictedMailSender is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedMailSender.AppId, privilege: $"{nameof(MailSender)}_read");
        }

        return unrestrictedMailSender;
    });

    public IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return mailSenderBroker.GetAllMailSenders(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newMailSender]);

        authorizationBroker.Authorize(appId: newMailSender.AppId, privilege: $"{nameof(MailSender)}_create");
        return await mailSenderBroker.AddMailSenderAsync(newMailSender: Copy(entity: newMailSender));
    }, isValueTask: true);

    public ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailSender]);

        authorizationBroker.Authorize(appId: updatedMailSender.AppId, privilege: $"{nameof(MailSender)}_update");
        return await mailSenderBroker.UpdateMailSenderAsync(updatedMailSender: Copy(entity: updatedMailSender));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailSenderId) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailSenderId]);

        MailSender entity = GetAllMailSender(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailSenderId);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_delete");
        return await mailSenderBroker.DeleteMailSenderAsync(deletedMailSender: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask DeleteAllMailSenderAsync(IEnumerable<MailSender> deletedMailSender) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [deletedMailSender]);

            return mailSenderBroker.DeleteAllMailSendersAsync(deletedMailSender: deletedMailSender);
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