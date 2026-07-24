// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailReceiverService
{
    MailReceiver GetMailReceiver(Guid iMailReceiverId);
    IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false);
    MailReceiver[] GetEnabled();
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverId);
    ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver);
    ValueTask DeleteAllByAppIdAsync(int appId);
}