// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Processings;

internal interface IMailReceivingProcessingService
{
    bool IsMigrationInProgress();

    void LogError(Exception exception);

    Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveTopAsync(
        Guid mailReceiverId,
        int count,
        CancellationToken cancellationToken = default);
}