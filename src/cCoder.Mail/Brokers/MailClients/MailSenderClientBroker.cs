// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures.MailClients;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailSenderClientBroker(IMailSenderFactory mailSenderFactory)
    : IMailSenderClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSenderFactory
            .GetSender(providerName: email?.MailSender?.ProviderName)
        .SendAsync(email: email, cancellationToken: cancellationToken);
}