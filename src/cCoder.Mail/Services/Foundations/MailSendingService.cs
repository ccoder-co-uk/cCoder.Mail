// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailSendingService(IMailSenderClientBroker mailSenderClientBroker) : IMailSendingService
{
    public Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateSendQueuedEmailAsync(inputs: [email, cancellationToken]);

            return mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);
        }, isTask: true);
}