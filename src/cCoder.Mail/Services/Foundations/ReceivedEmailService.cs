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
    public ReceivedEmail GetReceivedEmail(int receivedEmailId) =>
        TryCatch<ReceivedEmail>(operation: () =>
    {

        ValidateReceivedEmailOnGet(inputs: [receivedEmailId]);

        ReceivedEmail receivedEmail = receivedEmailBroker
            .GetAllReceivedEmails(ignoreFilters: false)
            .FirstOrDefault(predicate: item => item.Id == receivedEmailId);

        if (receivedEmail is not null)
        {
            return receivedEmail;
        }

        ReceivedEmail unrestrictedReceivedEmail = receivedEmailBroker
            .GetAllReceivedEmails(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == receivedEmailId);

        if (unrestrictedReceivedEmail is not null)
        {
            authorizationBroker.Authorize(appId: unrestrictedReceivedEmail.AppId, privilege: $"{nameof(ReceivedEmail)}_read");
        }

        return unrestrictedReceivedEmail;
    });

    public IQueryable<ReceivedEmail> GetAllReceivedEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateAllReceivedEmailOnGet(inputs: [ignoreFilters]);

            return receivedEmailBroker.GetAllReceivedEmails(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail newReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: async () =>
    {
        ValidateReceivedEmailOnAdd(inputs: [newReceivedEmail]);

        authorizationBroker.Authorize(appId: newReceivedEmail.AppId, privilege: $"{nameof(ReceivedEmail)}_create");
        return await receivedEmailBroker.AddReceivedEmailAsync(newReceivedEmail: Copy(entity: newReceivedEmail));
    }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail updatedReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: async () =>
    {
        ValidateReceivedEmailOnUpdate(inputs: [updatedReceivedEmail]);

        authorizationBroker.Authorize(appId: updatedReceivedEmail.AppId, privilege: $"{nameof(ReceivedEmail)}_update");
        return await receivedEmailBroker.UpdateReceivedEmailAsync(updatedReceivedEmail: Copy(entity: updatedReceivedEmail));
    }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int receivedEmailId) =>
        TryCatch<int>(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [receivedEmailId]);

        ReceivedEmail entity = receivedEmailBroker
            .GetAllReceivedEmails(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == receivedEmailId);

        authorizationBroker.Authorize(appId: entity.AppId, privilege: $"{nameof(ReceivedEmail)}_delete");
        return await receivedEmailBroker.DeleteReceivedEmailAsync(deletedReceivedEmail: Copy(entity: entity));
    }, isValueTask: true);

    public ValueTask AddRangeReceivedEmailAsync(
        IEnumerable<ReceivedEmail> newReceivedEmail,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateRangeReceivedEmailOnAdd(inputs: [newReceivedEmail, cancellationToken]);

            return receivedEmailBroker.AddReceivedEmailsAsync(newReceivedEmail: newReceivedEmail?.Select(selector: Copy), cancellationToken: cancellationToken);
        }, isValueTask: true);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        TryCatch<bool>(operation: () =>
        {
            ValidateExists(inputs: [mailReceiverId, messageId]);

            return receivedEmailBroker.Exists(mailReceiverId: mailReceiverId, messageId: messageId);
        });

    public ValueTask DeleteAllReceivedEmailAsync(IEnumerable<ReceivedEmail> deletedReceivedEmail) =>
        TryCatch(operation: () =>
        {
            ValidateAllReceivedEmailOnDelete(inputs: [deletedReceivedEmail]);

            return receivedEmailBroker.DeleteAllReceivedEmailsAsync(deletedReceivedEmail: deletedReceivedEmail);
        }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateAllByAppIdOnDelete(inputs: [appId]);

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