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
        QueuedEmail email,
        MailProviderConfiguration configuration,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.SendEmailAsync(
            email: email,
            configuration: configuration,
            cancellationToken: cancellationToken);

    public Task<HttpClientBrokerResponse> ReceiveEmailAsync(
        MailboxReceiveRequest request,
        MailProviderConfiguration configuration,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveEmailAsync(
            request: request,
            configuration: configuration,
            cancellationToken: cancellationToken);
}