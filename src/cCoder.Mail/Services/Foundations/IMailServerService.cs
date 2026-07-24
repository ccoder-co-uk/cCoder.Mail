// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface IMailServerService
{
    MailServer Get(int iMailServerId);
    IQueryable<MailServer> GetAll(bool ignoreFilters = false);
    ValueTask<MailServer> AddAsync(MailServer newMailServer);
    ValueTask<MailServer> UpdateAsync(MailServer updatedMailServer);
    ValueTask DeleteAsync(int iMailServerId);
    ValueTask DeleteAllForAppAsync(IEnumerable<MailServer> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}