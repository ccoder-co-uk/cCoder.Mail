// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailReceiverProcessingService(IMailReceiverService service) : IMailReceiverProcessingService
{
    public MailReceiver Get(Guid mailReceiverId) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateGet(inputs: [mailReceiverId]);

            return service.Get(iMailReceiverId: mailReceiverId);
        });

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public MailReceiver[] GetEnabled() =>
        TryCatch<MailReceiver[]>(operation: () =>
        {
            ValidateGetEnabled(inputs: []);

            return service.GetEnabled();
        });

    public ValueTask<MailReceiver> AddAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateAddAsync(inputs: [newMailReceiver]);

            return service.AddAsync(newMailReceiver: newMailReceiver);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedMailReceiver]);

            return service.UpdateAsync(updatedMailReceiver: updatedMailReceiver);
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

    public ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return service.DeleteAllAsync(items: items);
        }, isValueTask: true);
}