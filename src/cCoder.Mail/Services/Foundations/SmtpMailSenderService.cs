// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Mail;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {

        ValidateSendAsync(inputs: [email, cancellationToken]);

        MailSender sender = email.MailSender
                                                                                                                ?? throw new InvalidOperationException(message: "No mail sender configuration could be found to send the email.");

        using MailMessage message = CreateMailMessage(newQueuedEmail: email, newMailSender: sender);

        await smtpMailSenderBroker.SendAsync(
request: new SmtpMailSendRequest
{
    Host = sender.Host,
    Port = sender.Port,
    EnableSsl = sender.EnableSSL,
    User = sender.User,
    Password = sender.Password,
    Message = message,
},
cancellationToken: cancellationToken);
    }, isTask: true);

    private static MailMessage CreateMailMessage(QueuedEmail newQueuedEmail, MailSender newMailSender)
    {
        MailMessage message = new()
        {
            IsBodyHtml = newQueuedEmail.IsBodyHtml,
            Subject = newQueuedEmail.Subject,
            Body = newQueuedEmail.Content,
            From = CreateFromAddress(newMailSender: newMailSender),
        };

        message.To.Add(addresses: newQueuedEmail.To);

        if (!string.IsNullOrWhiteSpace(value: newQueuedEmail.CC))
        {
            message.CC.Add(addresses: newQueuedEmail.CC);
        }

        return message;
    }

    private static MailAddress CreateFromAddress(MailSender newMailSender)
    {
        if (!string.IsNullOrWhiteSpace(value: newMailSender.FromEmail))
        {
            return new MailAddress(address: newMailSender.FromEmail);
        }

        return newMailSender.User.Contains(value: '@')
            ? new MailAddress(address: newMailSender.User)
            : null;
    }
}