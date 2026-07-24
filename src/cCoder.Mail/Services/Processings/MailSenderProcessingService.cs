// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailSenderProcessingService(IMailSenderService service) : IMailSenderProcessingService
{
    public MailSender Get(Guid mailSenderId) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateGet(inputs: [mailSenderId]);

            return service.Get(iMailSenderId: mailSenderId);
        });

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateAddAsync(inputs: [newMailSender]);

            return service.AddAsync(newMailSender: newMailSender);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateAsync(MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedMailSender]);

            return service.UpdateAsync(updatedMailSender: updatedMailSender);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailSenderId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [mailSenderId]);

            return service.DeleteAsync(iMailSenderId: mailSenderId);
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