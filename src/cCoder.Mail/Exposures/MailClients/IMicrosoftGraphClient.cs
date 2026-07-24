// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Exposures.MailClients;

public interface IMicrosoftGraphClient
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default);
}