// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailReceiverProcessingService(IMailReceiverService service) : IMailReceiverProcessingService
{
    public MailReceiver GetMailReceiver(Guid mailReceiverId) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateGet(inputs: [mailReceiverId]);

            return service.GetMailReceiver(iMailReceiverId: mailReceiverId);
        });

    public IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAllMailReceiver(ignoreFilters: ignoreFilters);
        });

    public MailReceiver[] GetEnabled() =>
        TryCatch<MailReceiver[]>(operation: () =>
        {
            ValidateGetEnabled(inputs: []);

            return service.GetEnabled();
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateAddAsync(inputs: [newMailReceiver]);

            return service.AddMailReceiverAsync(newMailReceiver: newMailReceiver);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedMailReceiver]);

            return service.UpdateMailReceiverAsync(updatedMailReceiver: updatedMailReceiver);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailReceiverId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [mailReceiverId]);

            return service.DeleteAsync(iMailReceiverId: mailReceiverId);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [deletedMailReceiver]);

            return service.DeleteAllMailReceiverAsync(deletedMailReceiver: deletedMailReceiver);
        }, isValueTask: true);
}