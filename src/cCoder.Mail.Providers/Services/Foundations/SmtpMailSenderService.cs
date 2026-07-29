// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: () =>
            {
                ValidateSendQueuedEmailAsync(
                    inputs: [email, cancellationToken]);

                return smtpMailSenderBroker.SendAsync(
                    email: email,
                    cancellationToken: cancellationToken);
            },
            isTask: true);
}