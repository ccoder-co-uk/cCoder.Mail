// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IMailReceiverProcessingService
{
    MailReceiver GetMailReceiver(Guid iMailReceiverId);
    IQueryable<MailReceiver> GetAllMailReceivers(bool ignoreFilters = false);
    MailReceiver[] GetEnabled();
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);
    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);
    ValueTask<int> DeleteAsync(Guid iMailReceiverId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllMailReceiverAsync(IEnumerable<MailReceiver> deletedMailReceiver);
}