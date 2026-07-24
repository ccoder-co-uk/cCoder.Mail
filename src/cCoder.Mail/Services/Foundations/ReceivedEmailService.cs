// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        ReceivedEmail receivedEmail = GetAll()
            .FirstOrDefault(predicate: item => item.Id == id);

        if (receivedEmail is not null)
            return receivedEmail;

        ReceivedEmail unrestrictedReceivedEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (unrestrictedReceivedEmail is not null)
            authorizationBroker.Authorize(appId: unrestrictedReceivedEmail.AppId, privilege: $"{nameof(ReceivedEmail)}_read");

        return unrestrictedReceivedEmail;
    }

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        receivedEmailBroker.GetAllReceivedEmails(ignoreFilters: ignoreFilters);

    public async ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_create");
        return await receivedEmailBroker.AddReceivedEmailAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity)
    {
        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_update");
        return await receivedEmailBroker.UpdateReceivedEmailAsync(entity: Copy(entity: entity));
    }

    public async ValueTask<int> DeleteAsync(int id)
    {
        ReceivedEmail entity = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_delete");
        return await receivedEmailBroker.DeleteReceivedEmailAsync(entity: Copy(entity: entity));
    }

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        receivedEmailBroker.AddReceivedEmailsAsync(entities: entities?.Select(selector: Copy), cancellationToken: cancellationToken);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        receivedEmailBroker.Exists(mailReceiverId: mailReceiverId, messageId: messageId);

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) =>
        receivedEmailBroker.DeleteAllReceivedEmailsAsync(items: items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        receivedEmailBroker.DeleteAllReceivedEmailsByAppIdAsync(appId: appId);

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