// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Dependencies.MailClients;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class MicrosoftGraphBroker(
    MicrosoftGraphMailClientDependency mailClientDependency)
    : IMicrosoftGraphBroker
{
    public Task<(bool IsSuccessStatusCode, string Content)> SendEmailAsync(
        QueuedEmail queuedEmail,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.SendEmailAsync(
                email: queuedEmail,
                tenantId: tenantId,
                clientId: clientId,
                clientSecret: clientSecret,
                graphBaseUrl: graphBaseUrl,
                loginBaseUrl: loginBaseUrl,
                cancellationToken: cancellationToken);

    public Task<(bool IsSuccessStatusCode, string Content)> ReceiveEmailAsync(
        string user,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int maximumMessages,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.ReceiveEmailAsync(
                user: user,
                from: from,
                to: to,
                maximumMessages: maximumMessages,
                tenantId: tenantId,
                clientId: clientId,
                clientSecret: clientSecret,
                graphBaseUrl: graphBaseUrl,
                loginBaseUrl: loginBaseUrl,
                cancellationToken: cancellationToken);
}