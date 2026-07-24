// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IMailSenderProcessingService
{
    MailSender GetMailSender(Guid iMailSenderId);
    IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false);
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllMailSenderAsync(IEnumerable<MailSender> items);
}