// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailSenderService
{
    MailSender Get(Guid iMailSenderId);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderId);
    ValueTask DeleteAllAsync(IEnumerable<MailSender> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}