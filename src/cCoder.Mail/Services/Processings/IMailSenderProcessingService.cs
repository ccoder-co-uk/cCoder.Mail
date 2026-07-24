// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IMailSenderProcessingService
{
    MailSender Get(Guid iMailSenderId);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllAsync(IEnumerable<MailSender> items);
}