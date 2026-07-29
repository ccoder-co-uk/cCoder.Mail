// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

public interface IMicrosoftGraphBroker
{
    Task<HttpClientBrokerResponse> SendEmailAsync(
        QueuedEmail email,
        MicrosoftGraphProviderConfiguration configuration,
        CancellationToken cancellationToken = default);

    Task<HttpClientBrokerResponse> ReceiveEmailAsync(
        MailboxReceiveRequest request,
        MicrosoftGraphProviderConfiguration configuration,
        CancellationToken cancellationToken = default);
}