// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Dependencies.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class Pop3MailReceiverBroker(
    Pop3MailClientDependency mailClientDependency)
    : IPop3MailReceiverBroker
{
    public Task<string[][]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveAsync(
            request: request,
            cancellationToken: cancellationToken);
}