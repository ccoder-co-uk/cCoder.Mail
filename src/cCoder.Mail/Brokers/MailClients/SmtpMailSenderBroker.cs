// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class SmtpMailSenderBroker : ISmtpMailSenderBroker
{
    public async Task SendAsync(SmtpMailSendRequest request, CancellationToken cancellationToken = default)
    {
        using SmtpClient client = new()
        {
            Host = request.Host,
            Port = request.Port,
            EnableSsl = request.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(userName: request.User, password: request.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        await client.SendMailAsync(message: request.Message, cancellationToken: cancellationToken);
    }
}