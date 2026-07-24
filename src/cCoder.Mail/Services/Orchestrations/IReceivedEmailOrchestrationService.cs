// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Orchestrations;

public interface IReceivedEmailOrchestrationService
{
    ReceivedEmail Get(int id);
    IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity);
    ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity);
    ValueTask<int> DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);
    Task<ReceivedEmail[]> ReceiveAsync(MailboxReceiveRequest request, CancellationToken cancellationToken = default);
    Task<ReceivedEmail[]> ReceiveTopAsync(int count, CancellationToken cancellationToken = default);
}