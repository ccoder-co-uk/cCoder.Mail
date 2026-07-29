// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailSenderClientBroker(IMailClientFactory mailClientFactory)
    : IMailSenderClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailClientFactory
            .CreateMailClient(
                providerName:
                    email?.MailSender?.ProviderName)
        .SendAsync(email: email, cancellationToken: cancellationToken);
}