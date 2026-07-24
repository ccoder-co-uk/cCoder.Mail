// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailSenderConfigurationOrchestrationService
{
    MailSender Get(Guid iMailSenderConfigurationId);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderConfigurationId);
    ValueTask DeleteByAppIdAsync(int appId);
}