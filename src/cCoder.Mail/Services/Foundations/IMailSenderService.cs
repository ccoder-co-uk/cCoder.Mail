// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

internal interface IMailSenderService
{
    MailSender GetMailSender(Guid iMailSenderId);
    IQueryable<MailSender> GetAllMailSenders(bool ignoreFilters = false);
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask<int> DeleteAsync(Guid iMailSenderId);
    ValueTask DeleteAllMailSenderAsync(IEnumerable<MailSender> deletedMailSender);
    ValueTask DeleteAllByAppIdAsync(int appId);
}