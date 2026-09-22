// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Dependencies.MailClients;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class SmtpMailSenderBroker(
    SmtpMailClientDependency mailClientDependency)
    : ISmtpMailSenderBroker
{
    public Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default) =>
        mailClientDependency.SendAsync(
            email: queuedEmail,
            cancellationToken: cancellationToken);
}