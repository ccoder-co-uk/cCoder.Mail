// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public Task SendQueuedEmailAsync(QueuedEmail queuedEmail, CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: () =>
            {
                ValidateSendQueuedEmailAsync(
                    inputs: [queuedEmail, cancellationToken]);

                return smtpMailSenderBroker.SendAsync(
                    queuedEmail: queuedEmail,
                    cancellationToken: cancellationToken);
            },
            isTask: true);
}