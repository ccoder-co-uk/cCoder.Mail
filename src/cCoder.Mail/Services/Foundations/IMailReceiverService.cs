// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailReceiverService
{
    MailReceiver Get(Guid iMailReceiverId);
    IQueryable<MailReceiver> GetAll(bool ignoreFilters = false);
    MailReceiver[] GetEnabled();
    ValueTask<MailReceiver> AddAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverId);
    ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}