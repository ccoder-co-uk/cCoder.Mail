// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IMailSenderProcessingService
{
    MailSender Get(Guid id);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender entity);
    ValueTask<MailSender> UpdateAsync(MailSender entity);
    ValueTask<int> DeleteAsync(Guid id);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllAsync(IEnumerable<MailSender> items);
}