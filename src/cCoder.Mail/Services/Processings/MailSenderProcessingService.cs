// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailSenderProcessingService(IMailSenderService service) : IMailSenderProcessingService
{
    public MailSender GetMailSender(Guid mailSenderId) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnGet(inputs: [mailSenderId]);

            return service.GetMailSender(iMailSenderId: mailSenderId);
        });

    public IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateAllMailSenderOnGet(inputs: [ignoreFilters]);

            return service.GetAllMailSender(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnAdd(inputs: [newMailSender]);

            return service.AddMailSenderAsync(newMailSender: newMailSender);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnUpdate(inputs: [updatedMailSender]);

            return service.UpdateMailSenderAsync(updatedMailSender: updatedMailSender);
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
            ValidateByAppIdOnDelete(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask DeleteAllMailSenderAsync(IEnumerable<MailSender> deletedMailSender) =>
        TryCatch(operation: () =>
        {
            ValidateAllMailSenderOnDelete(inputs: [deletedMailSender]);

            return service.DeleteAllMailSenderAsync(deletedMailSender: deletedMailSender);
        }, isValueTask: true);
}