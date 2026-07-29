// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Dependencies.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class ImapMailReceiverBroker(
    ImapMailClientDependency mailClientDependency)
    : IImapMailReceiverBroker
{
    public Task<string[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveAsync(
            request: request,
            cancellationToken: cancellationToken);
}