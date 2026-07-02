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
            Credentials = new NetworkCredential(request.User, request.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        using MailMessage message = new()
        {
            IsBodyHtml = request.IsBodyHtml,
            Subject = request.Subject,
            Body = request.Content
        };

        if (!string.IsNullOrWhiteSpace(request.From))
            message.From = new MailAddress(request.From);

        message.From ??= request.User.Contains('@')
            ? new MailAddress(request.User)
            : null;

        message.To.Add(request.To);

        if (!string.IsNullOrWhiteSpace(request.CC))
            message.CC.Add(request.CC);

        await client.SendMailAsync(message, cancellationToken);
    }
}
