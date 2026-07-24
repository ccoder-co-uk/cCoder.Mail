// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailSenderConfigurationOrchestrationService
{
    MailSender GetMailSender(Guid iMailSenderConfigurationId);
    IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false);
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderConfigurationId);
    ValueTask DeleteByAppIdAsync(int appId);
}