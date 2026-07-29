// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Exposures.MailClients;

public interface IMailClient
{
    string[] GetProviderNames();

    MailClientOperation[] GetSupportedOperations();

    Task SendAsync(
        QueuedEmail email,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default);
}