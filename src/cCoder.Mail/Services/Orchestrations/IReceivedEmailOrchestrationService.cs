// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Orchestrations;

internal interface IReceivedEmailOrchestrationService
{
    ValueTask<bool> ExistsAsync(int receivedEmailId);

    ValueTask<ReceivedEmail> AddReceivedEmailAsync(
        ReceivedEmail newReceivedEmail);

    ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(
        ReceivedEmail updatedReceivedEmail);

    ValueTask DeleteByAppIdAsync(int appId);

    Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveTopAsync(
        Guid mailReceiverId,
        int count,
        CancellationToken cancellationToken = default);
}