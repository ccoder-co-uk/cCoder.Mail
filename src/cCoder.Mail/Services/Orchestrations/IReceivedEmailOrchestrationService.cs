// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Orchestrations;

public interface IReceivedEmailOrchestrationService
{
    ReceivedEmail GetReceivedEmail(int iReceivedEmailId);
    IQueryable<ReceivedEmail> GetAllReceivedEmail(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail newReceivedEmail);
    ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail updatedReceivedEmail);
    ValueTask<int> DeleteAsync(int iReceivedEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(MailboxReceiveRequest request, CancellationToken cancellationToken = default);
    Task<ReceivedEmail[]> ReceiveTopAsync(int count, CancellationToken cancellationToken = default);
}