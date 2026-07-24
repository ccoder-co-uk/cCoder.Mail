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
            ValidateMailReceiverOnGet(inputs: [mailReceiverId]);

            return service.GetMailReceiver(iMailReceiverId: mailReceiverId);
        });

    public IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateAllMailReceiverOnGet(inputs: [ignoreFilters]);

            return service.GetAllMailReceiver(ignoreFilters: ignoreFilters);
        });

    public MailReceiver[] GetEnabled() =>
        TryCatch<MailReceiver[]>(operation: () =>
        {
            ValidateEnabledOnGet(inputs: []);

            return service.GetEnabled();
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateMailReceiverOnAdd(inputs: [newMailReceiver]);

            return service.AddMailReceiverAsync(newMailReceiver: newMailReceiver);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateMailReceiverOnUpdate(inputs: [updatedMailReceiver]);

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
            ValidateByAppIdOnDelete(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver) =>
        TryCatch(operation: () =>
        {
            ValidateAllMailReceiverOnDelete(inputs: [deletedMailReceiver]);

            return service.DeleteAllMailReceiverAsync(deletedMailReceiver: deletedMailReceiver);
        }, isValueTask: true);
}