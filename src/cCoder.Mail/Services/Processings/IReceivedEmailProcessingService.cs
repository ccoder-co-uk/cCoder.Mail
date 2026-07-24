// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IReceivedEmailProcessingService
{
    ReceivedEmail Get(int id);
    IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity);
    ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity);
    ValueTask<int> DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask AddRangeAsync(IEnumerable<ReceivedEmail> entities, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items);
}