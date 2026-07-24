// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailReceiverConfigurationOrchestrationService
{
    MailReceiver GetMailReceiver(Guid iMailReceiverConfigurationId);
    IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false);
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverConfigurationId);
    ValueTask DeleteByAppIdAsync(int appId);
}