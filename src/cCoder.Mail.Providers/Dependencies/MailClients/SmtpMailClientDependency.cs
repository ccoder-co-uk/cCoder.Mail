// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Mail;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Dependencies.MailClients;

internal sealed class SmtpMailClientDependency : SmtpClient
{
    internal async Task SendAsync(
        QueuedEmail email,
        CancellationToken cancellationToken = default)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException(
                message:
                    "No mail sender configuration could be found to send the email.");

        Host = sender.Host;
        Port = sender.Port;
        EnableSsl = sender.EnableSSL;
        UseDefaultCredentials = false;
        Credentials = new NetworkCredential(
            userName: sender.User,
            password: sender.Password);
        DeliveryMethod = SmtpDeliveryMethod.Network;

        using MailMessage message = CreateMailMessage(
            email: email,
            sender: sender);

        await SendMailAsync(
            message: message,
            cancellationToken: cancellationToken);
    }

    private static MailMessage CreateMailMessage(
        QueuedEmail email,
        MailSender sender)
    {
        MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content,
            From = CreateMailAddress(sender: sender),
        };

        message.To.Add(addresses: email.To);

        if (!string.IsNullOrWhiteSpace(value: email.CC))
        {
            message.CC.Add(addresses: email.CC);
        }

        return message;
    }

    private static MailAddress CreateMailAddress(
        MailSender sender)
    {
        if (!string.IsNullOrWhiteSpace(value: sender.FromEmail))
        {
            return new MailAddress(address: sender.FromEmail);
        }

        return sender.User.Contains(value: '@')
            ? new MailAddress(address: sender.User)
            : null;
    }
}