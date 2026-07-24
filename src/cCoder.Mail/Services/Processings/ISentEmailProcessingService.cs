// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Processings;

public interface ISentEmailProcessingService
{
    SentEmail GetSentEmail(int iSentEmailId);

    IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false);

    ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail);

    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail);

    ValueTask DeleteAsync(int iSentEmailId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdateSentEmailResult(IEnumerable<SentEmail> items);

    ValueTask DeleteAllSentEmailAsync(IEnumerable<SentEmail> items);
}