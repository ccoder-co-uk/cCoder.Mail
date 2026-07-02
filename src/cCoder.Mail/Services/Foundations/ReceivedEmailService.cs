using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal class ReceivedEmailService(
    IReceivedEmailBroker receivedEmailBroker,
    IAuthorizationBroker authorizationBroker)
    : IReceivedEmailService
{
    public ReceivedEmail Get(int id)
    {
        ReceivedEmail receivedEmail = GetAll().FirstOrDefault(item => item.Id == id);

        if (receivedEmail is not null)
            return receivedEmail;

        ReceivedEmail unrestrictedReceivedEmail = GetAll(true).FirstOrDefault(item => item.Id == id);

        if (unrestrictedReceivedEmail is not null)
            authorizationBroker.Authorize(unrestrictedReceivedEmail.AppId, $"{nameof(ReceivedEmail)}_read");

        return unrestrictedReceivedEmail;
    }

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        receivedEmailBroker.GetAllReceivedEmails(ignoreFilters);

    public async ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity)
    {
        authorizationBroker.Authorize(entity.AppId, $"{nameof(ReceivedEmail)}_create");
        return await receivedEmailBroker.AddReceivedEmailAsync(Copy(entity));
    }

    public async ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity)
    {
        authorizationBroker.Authorize(entity.AppId, $"{nameof(ReceivedEmail)}_update");
        return await receivedEmailBroker.UpdateReceivedEmailAsync(Copy(entity));
    }

    public async ValueTask<int> DeleteAsync(int id)
    {
        ReceivedEmail entity = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);
        authorizationBroker.Authorize(entity.AppId, $"{nameof(ReceivedEmail)}_delete");
        return await receivedEmailBroker.DeleteReceivedEmailAsync(Copy(entity));
    }

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        receivedEmailBroker.AddReceivedEmailsAsync(entities?.Select(Copy), cancellationToken);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        receivedEmailBroker.Exists(mailReceiverId, messageId);

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) =>
        receivedEmailBroker.DeleteAllReceivedEmailsAsync(items);

    private static ReceivedEmail Copy(ReceivedEmail entity) =>
        entity is null
            ? null
            : new()
            {
                Id = entity.Id,
                AppId = entity.AppId,
                SentByUserId = entity.SentByUserId,
                Subject = entity.Subject,
                Content = entity.Content,
                To = entity.To,
                CC = entity.CC,
                IsBodyHtml = entity.IsBodyHtml,
                ReceivedOn = entity.ReceivedOn,
                From = entity.From,
                MessageId = entity.MessageId,
                MailReceiverId = entity.MailReceiverId,
            };
}
