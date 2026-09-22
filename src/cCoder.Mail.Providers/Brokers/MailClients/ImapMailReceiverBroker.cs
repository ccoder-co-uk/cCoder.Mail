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
        MailboxReceiveRequest mailboxReceiveRequest,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveAsync(
            host: mailboxReceiveRequest.Host,
            port: mailboxReceiveRequest.Port,
            enableSsl: mailboxReceiveRequest.EnableSSL,
            user: mailboxReceiveRequest.User,
            password: mailboxReceiveRequest.Password,
            from: mailboxReceiveRequest.From,
            maximumMessages: mailboxReceiveRequest.MaximumMessages,
            cancellationToken: cancellationToken);
}