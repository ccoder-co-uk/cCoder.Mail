// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Exposures;

public interface IMailReceiverManager
{
    MailReceiver GetMailReceiver(Guid iMailReceiverId);
    IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false);
    MailReceiver[] GetEnabled();
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver);
}
