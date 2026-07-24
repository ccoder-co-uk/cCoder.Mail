// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal class MailSenderService(
    IMailSenderBroker mailSenderBroker,
    IAuthorizationBroker authorizationBroker)
    : IMailSenderService
{
    public MailSender Get(Guid id)
    {
        MailSender mailSender = GetAll()
            .FirstOrDefault(predicate: item => item.Id == id);

        if (mailSender is not null)
            return mailSender;

        MailSender unrestrictedMailSender = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (unrestrictedMailSender is not null)
            authorizationBroker.Authorize(appId: unrestrictedMailSender.AppId, privilege: $"{nameof(MailSender)}_read");

        return unrestrictedMailSender;
    }

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        mailSenderBroker.GetAllMailSenders(ignoreFilters: ignoreFilters);

    public async ValueTask<MailSender> AddAsync(MailSender entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_create");
        return await mailSenderBroker.AddMailSenderAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<MailSender> UpdateAsync(MailSender entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_update");
        return await mailSenderBroker.UpdateMailSenderAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<int> DeleteAsync(Guid id)
    {
        MailSender entity = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(MailSender)}_delete");
        return await mailSenderBroker.DeleteMailSenderAsync(entity: Copy(entity: entity));
    }

    public ValueTask DeleteAllAsync(IEnumerable<MailSender> items) =>
        mailSenderBroker.DeleteAllMailSendersAsync(items: items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        mailSenderBroker.DeleteAllMailSendersByAppIdAsync(appId: appId);

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