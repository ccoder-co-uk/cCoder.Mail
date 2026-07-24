// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;

namespace cCoder.Mail.Services.Foundations;

internal partial class ReceivedEmailService(
    IReceivedEmailBroker receivedEmailBroker,
    IAuthorizationBroker authorizationBroker)
    : IReceivedEmailService
{
    public ReceivedEmail Get(int id) =>
        TryCatch<ReceivedEmail>(operation: () =>
    {

        ValidateGet(inputs: [id]);

        ReceivedEmail receivedEmail = GetAll()
                                               .FirstOrDefault(predicate: item => item.Id == id);

        if (receivedEmail is not null)
        {
            return receivedEmail;
        }

        ReceivedEmail unrestrictedReceivedEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (unrestrictedReceivedEmail is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedReceivedEmail.AppId, privilege: $"{nameof(ReceivedEmail)}_read");
        }

        return unrestrictedReceivedEmail;
    });

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return receivedEmailBroker.GetAllReceivedEmails(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_create");
        return await receivedEmailBroker.AddReceivedEmailAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_update");
        return await receivedEmailBroker.UpdateReceivedEmailAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int id) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        ReceivedEmail entity = GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_delete");
        return await receivedEmailBroker.DeleteReceivedEmailAsync(entity: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateAddRangeAsync(inputs: [entities, cancellationToken]);

            return receivedEmailBroker.AddReceivedEmailsAsync(entities: entities?.Select(selector: Copy), cancellationToken: cancellationToken);
        }, isValueTask: true);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        TryCatch<bool>(operation: () =>
        {
            ValidateExists(inputs: [mailReceiverId, messageId]);

            return receivedEmailBroker.Exists(mailReceiverId: mailReceiverId, messageId: messageId);
        });

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return receivedEmailBroker.DeleteAllReceivedEmailsAsync(items: items);
        }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllByAppIdAsync(inputs: [appId]);

            return receivedEmailBroker.DeleteAllReceivedEmailsByAppIdAsync(appId: appId);
        }, isValueTask: true);

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