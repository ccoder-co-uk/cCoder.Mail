// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailServerOrchestrationService
{
    MailServer Get(int id);

    IQueryable<MailServer> GetAll(bool ignoreFilters = false);

    ValueTask<MailServer> AddAsync(MailServer entity);

    ValueTask<MailServer> UpdateAsync(MailServer entity);

    ValueTask DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items);

    ValueTask DeleteAllAsync(IEnumerable<MailServer> items);
}