// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class SmtpMailClient(
    ISmtpMailSenderService smtpMailSenderService)
    : IMailClient
{
    public string[] GetProviderNames() =>
        [MailProviderNames.Smtp];

    public MailClientOperation[] GetSupportedOperations() =>
        [MailClientOperation.Send];

    public Task SendAsync(
        QueuedEmail email,
        CancellationToken cancellationToken = default) =>
        smtpMailSenderService.SendQueuedEmailAsync(
            email: email,
            cancellationToken: cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        throw new UnsupportedMailClientOperationException(
            providerName: MailProviderNames.Smtp,
            operation: "Receive");
}