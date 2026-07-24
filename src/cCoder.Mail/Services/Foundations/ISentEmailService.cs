// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface ISentEmailService
{
    SentEmail Get(int iSentEmailId);
    IQueryable<SentEmail> GetAll(bool ignoreFilters = false);
    ValueTask<SentEmail> AddAsync(SentEmail newSentEmail);
    ValueTask<SentEmail> UpdateAsync(SentEmail updatedSentEmail);
    ValueTask DeleteAsync(int iSentEmailId);
    ValueTask DeleteAllForAppAsync(IEnumerable<SentEmail> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}