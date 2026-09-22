// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal interface IMicrosoftGraphBroker
{
    Task<(bool IsSuccessStatusCode, string Content)> SendEmailAsync(
        QueuedEmail email,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default);

    Task<(bool IsSuccessStatusCode, string Content)> ReceiveEmailAsync(
        string user,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int maximumMessages,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default);
}