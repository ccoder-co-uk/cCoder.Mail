// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailSenderService
{
    MailSender Get(Guid id);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender entity);
    ValueTask<MailSender> UpdateAsync(MailSender entity);
    ValueTask<int> DeleteAsync(Guid id);
    ValueTask DeleteAllAsync(IEnumerable<MailSender> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}