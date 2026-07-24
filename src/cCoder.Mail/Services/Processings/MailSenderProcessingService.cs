// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailSenderProcessingService(IMailSenderService service) : IMailSenderProcessingService
{
    public MailSender Get(Guid id) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return service.Get(id: id);
        });

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return service.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: () =>
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

    public ValueTask DeleteAllAsync(IEnumerable<MailSender> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return service.DeleteAllAsync(items: items);
        }, isValueTask: true);
}