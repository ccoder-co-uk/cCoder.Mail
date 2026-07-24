// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Mail;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public async Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException(message: "No mail sender configuration could be found to send the email.");

        using MailMessage message = CreateMailMessage(email: email, sender: sender);

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
    }

    private static MailMessage CreateMailMessage(QueuedEmail email, MailSender sender)
    {
        MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content,
            From = CreateFromAddress(sender: sender),
        };

        message.To.Add(addresses: email.To);

        if (!string.IsNullOrWhiteSpace(value: email.CC))
            message.CC.Add(addresses: email.CC);

        return message;
    }

    private static MailAddress CreateFromAddress(MailSender sender)
    {
        if (!string.IsNullOrWhiteSpace(value: sender.FromEmail))
            return new MailAddress(address: sender.FromEmail);

        return sender.User.Contains(value: '@')
            ? new MailAddress(address: sender.User)
            : null;
    }
}