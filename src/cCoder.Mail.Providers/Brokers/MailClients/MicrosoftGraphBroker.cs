// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Dependencies.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class MicrosoftGraphBroker(
    MicrosoftGraphMailClientDependency mailClientDependency)
    : IMicrosoftGraphBroker
{
    public Task<HttpClientBrokerResponse> SendEmailAsync(
        QueuedEmail queuedEmail,
        MailProviderConfiguration mailProviderConfiguration,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.SendEmailAsync(
            email: queuedEmail,
            configuration: mailProviderConfiguration,
            cancellationToken: cancellationToken);

    public Task<HttpClientBrokerResponse> ReceiveEmailAsync(
        MailboxReceiveRequest mailboxReceiveRequest,
        MailProviderConfiguration mailProviderConfiguration,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveEmailAsync(
            request: mailboxReceiveRequest,
            configuration: mailProviderConfiguration,
            cancellationToken: cancellationToken);
}