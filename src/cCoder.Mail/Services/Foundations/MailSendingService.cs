// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;

namespace cCoder.Mail.Services.Foundations;

internal sealed class MailSendingService(IMailSenderClientBroker mailSenderClientBroker) : IMailSendingService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);
}