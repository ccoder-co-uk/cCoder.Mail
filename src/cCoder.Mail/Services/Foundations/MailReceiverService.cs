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
        MailReceiver mailReceiver = GetAll().FirstOrDefault(item => item.Id == id);

        if (mailReceiver is not null)
            return mailReceiver;

        MailReceiver unrestrictedMailReceiver = GetAll(true).FirstOrDefault(item => item.Id == id);

        if (unrestrictedMailReceiver is not null)
            authorizationBroker.Authorize(unrestrictedMailReceiver.AppId, $"{nameof(MailReceiver)}_read");

        return unrestrictedMailReceiver;
    }

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
        mailReceiverBroker.GetAllMailReceivers(ignoreFilters);

    public MailReceiver[] GetEnabled() => mailReceiverBroker.GetEnabledMailReceivers();

    public async ValueTask<MailReceiver> AddAsync(MailReceiver entity)
    {
        authorizationBroker.Authorize(entity.AppId, $"{nameof(MailReceiver)}_create");
        return await mailReceiverBroker.AddMailReceiverAsync(Copy(entity));
    }

    public async ValueTask<MailReceiver> UpdateAsync(MailReceiver entity)
    {
        authorizationBroker.Authorize(entity.AppId, $"{nameof(MailReceiver)}_update");
        return await mailReceiverBroker.UpdateMailReceiverAsync(Copy(entity));
    }

    public async ValueTask<int> DeleteAsync(Guid id)
    {
        MailReceiver entity = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);
        authorizationBroker.Authorize(entity.AppId, $"{nameof(MailReceiver)}_delete");
        return await mailReceiverBroker.DeleteMailReceiverAsync(Copy(entity));
    }

    public ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items) =>
        mailReceiverBroker.DeleteAllMailReceiversAsync(items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        mailReceiverBroker.DeleteAllMailReceiversByAppIdAsync(appId);

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
