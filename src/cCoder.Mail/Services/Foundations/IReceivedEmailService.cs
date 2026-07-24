// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IReceivedEmailService
{
    ReceivedEmail Get(int iReceivedEmailId);
    IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddAsync(ReceivedEmail newReceivedEmail);
    ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail updatedReceivedEmail);
    ValueTask<int> DeleteAsync(int iReceivedEmailId);
    ValueTask AddRangeAsync(IEnumerable<ReceivedEmail> entities, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}