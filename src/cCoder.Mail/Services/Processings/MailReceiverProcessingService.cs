// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailReceiverProcessingService(IMailReceiverService service) : IMailReceiverProcessingService
{
    public MailReceiver Get(Guid id) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return service.Get(id: id);
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

    public ValueTask<MailReceiver> AddAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return service.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [entity]);

            return service.UpdateAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid id) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [id]);

            return service.DeleteAsync(id: id);
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