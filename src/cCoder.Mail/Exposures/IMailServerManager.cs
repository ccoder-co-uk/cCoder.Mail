// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

public interface IMailServerManager
{
    MailServer GetMailServer(int iMailServerId);
    IQueryable<MailServer> GetAllMailServer(bool ignoreFilters = false);
    ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer);
    ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer);
    ValueTask DeleteAsync(int iMailServerId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdateMailServerResult(IEnumerable<MailServer> newMailServer);
    ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> deletedMailServer);
}
