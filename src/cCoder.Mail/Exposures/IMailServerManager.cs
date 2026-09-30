// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

public interface IMailServerManager
{
    MailServer GetMailServer(int iMailServerId);
    IQueryable<MailServer> GetAllMailServers(bool ignoreFilters = false);
    ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer);
    ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer);
    ValueTask DeleteAsync(int iMailServerId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdateMailServerResults(IEnumerable<MailServer> newMailServer);
    ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> deletedMailServer);
}