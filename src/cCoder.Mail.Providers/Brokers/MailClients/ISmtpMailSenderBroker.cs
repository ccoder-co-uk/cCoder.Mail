// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Brokers.MailClients;

public interface ISmtpMailSenderBroker
{
    Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default);
}