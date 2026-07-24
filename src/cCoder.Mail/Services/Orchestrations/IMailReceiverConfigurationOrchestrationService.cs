// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailReceiverConfigurationOrchestrationService
{
    MailReceiver Get(Guid iMailReceiverConfigurationId);
    IQueryable<MailReceiver> GetAll(bool ignoreFilters = false);
    ValueTask<MailReceiver> AddAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverConfigurationId);
    ValueTask DeleteByAppIdAsync(int appId);
}