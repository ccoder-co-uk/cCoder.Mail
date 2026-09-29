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

internal interface ISentEmailService
{
    SentEmail GetSentEmail(int iSentEmailId);
    IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false);
    ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail);
    ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail);
    ValueTask DeleteAsync(int iSentEmailId);
    ValueTask DeleteAllForAppSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail);
    ValueTask DeleteAllByAppIdAsync(int appId);
}