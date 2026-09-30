// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Exposures;

public interface IReceivedEmailManager
{
    ReceivedEmail GetReceivedEmail(int iReceivedEmailId);
    IQueryable<ReceivedEmail> GetAllReceivedEmails(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail newReceivedEmail);
    ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail updatedReceivedEmail);
    ValueTask<int> DeleteAsync(int iReceivedEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask AddRangeReceivedEmailAsync(IEnumerable<ReceivedEmail> newReceivedEmail, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllReceivedEmailAsync(IEnumerable<ReceivedEmail> deletedReceivedEmail);
}