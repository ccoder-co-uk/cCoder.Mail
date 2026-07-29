// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

public interface IImapMailReceiverBroker
{
    Task<string[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}