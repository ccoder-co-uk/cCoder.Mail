// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

internal interface IMailServerService
{
    MailServer GetMailServer(int iMailServerId);
    IQueryable<MailServer> GetAllMailServers(bool ignoreFilters = false);
    ValueTask<MailServer> AddMailServerAsync(
        MailServer newMailServer,
        bool checkPrivileges = true);
    ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer);
    ValueTask DeleteAsync(int iMailServerId);
    ValueTask DeleteAllForAppMailServerAsync(IEnumerable<MailServer> deletedMailServer);
    ValueTask DeleteAllByAppIdAsync(int appId);
}