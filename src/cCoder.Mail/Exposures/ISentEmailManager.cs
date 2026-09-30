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

public interface ISentEmailManager
{
    SentEmail GetSentEmail(int iSentEmailId);
    IQueryable<SentEmail> GetAllSentEmails(bool ignoreFilters = false);
    ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail);
    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail);
    ValueTask DeleteAsync(int iSentEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdateSentEmailResults(IEnumerable<SentEmail> newSentEmail);
    ValueTask DeleteAllSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail);
}