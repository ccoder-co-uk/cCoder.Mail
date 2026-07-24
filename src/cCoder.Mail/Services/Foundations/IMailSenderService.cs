// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailSenderService
{
    MailSender GetMailSender(Guid iMailSenderId);
    IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false);
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderId);
    ValueTask DeleteAllMailSenderAsync(IEnumerable<MailSender> deletedMailSender);
    ValueTask DeleteAllByAppIdAsync(int appId);
}