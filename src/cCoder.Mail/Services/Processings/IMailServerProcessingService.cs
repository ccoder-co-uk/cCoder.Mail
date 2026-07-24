// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Processings;

public interface IMailServerProcessingService
{
    MailServer Get(int iMailServerId);

    IQueryable<MailServer> GetAll(bool ignoreFilters = false);

    ValueTask<MailServer> AddAsync(MailServer newMailServer);

    ValueTask<MailServer> UpdateAsync(MailServer updatedMailServer);

    ValueTask DeleteAsync(int iMailServerId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items);

    ValueTask DeleteAllAsync(IEnumerable<MailServer> items);
}