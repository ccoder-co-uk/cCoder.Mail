// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

public interface IPop3MailReceiverBroker
{
    Task<string[][]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}