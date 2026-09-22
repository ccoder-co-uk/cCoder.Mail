// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal interface IImapMailReceiverBroker
{
    Task<string[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}