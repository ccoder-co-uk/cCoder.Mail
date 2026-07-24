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
    MailServer GetMailServer(int iMailServerId);

    IQueryable<MailServer> GetAllMailServer(bool ignoreFilters = false);

    ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer);

    ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer);

    ValueTask DeleteAsync(int iMailServerId);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdateMailServerResult(IEnumerable<MailServer> items);

    ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> items);
}