// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

public interface ISentEmailManager
{
    SentEmail GetSentEmail(int iSentEmailId);
    IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false);
    ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail);
    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail);
    ValueTask DeleteAsync(int iSentEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdateSentEmailResult(IEnumerable<SentEmail> newSentEmail);
    ValueTask DeleteAllSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail);
}
