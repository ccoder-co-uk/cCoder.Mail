// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class MailSenderProcessingService(IMailSenderService service) : IMailSenderProcessingService
{
    public MailSender Get(Guid id) =>
        service.Get(id: id);

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<MailSender> AddAsync(MailSender entity) =>
        service.AddAsync(entity: entity);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) =>
        service.UpdateAsync(entity: entity);

    public ValueTask<int> DeleteAsync(Guid id) =>
        service.DeleteAsync(id: id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public ValueTask DeleteAllAsync(IEnumerable<MailSender> items) =>
        service.DeleteAllAsync(items: items);
}