// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IReceivedEmailProcessingService
{
    ReceivedEmail GetReceivedEmail(int iReceivedEmailId);
    IQueryable<ReceivedEmail> GetAllReceivedEmail(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail newReceivedEmail);
    ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail updatedReceivedEmail);
    ValueTask<int> DeleteAsync(int iReceivedEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask AddRangeReceivedEmailAsync(IEnumerable<ReceivedEmail> entities, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllReceivedEmailAsync(IEnumerable<ReceivedEmail> items);
}