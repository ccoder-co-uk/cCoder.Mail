using System.Net;
using System.Net.Mail;
using System.Text;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;


namespace cCoder.Mail.Services.Orchestrations;

internal sealed class MailSenderOrchestrationService(
    IQueuedEmailService queuedEmailService,
    ILogger<MailSenderOrchestrationService> log)
    : IMailSenderOrchestrationService
{
    public async Task RunContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await RunAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        QueuedEmail[] queue = queuedEmailService.GetDispatchBatch(batchSize: 10, maxFailures: 10);

        if (queue.Length == 0)
            return;

        log.LogInformation("Picked up a batch of {Count} emails.", queue.Length);

        int success = 0;
        int failures = 0;

        using SmtpClient client = new() { EnableSsl = true };

        foreach (QueuedEmail email in queue)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await ProcessEmailAsync(client, email, cancellationToken))
                success++;
            else
                failures++;

            await Task.Delay(500, cancellationToken);
        }

        log.LogInformation(
            "{Count} SMTP requests made of which {Success} succeeded and {Failures} failed.",
            success + failures,
            success,
            failures);
    }

    private async Task<bool> ProcessEmailAsync(
        SmtpClient client,
        QueuedEmail email,
        CancellationToken cancellationToken)
    {
        MailServer server = email.App?.MailServers?.FirstOrDefault(mailServer => mailServer.Name == email.MailServerName);

        if (server == null)
        {
            await queuedEmailService.RecordSendFailureAsync(
                email.Id,
                "No mail server configuration could be found to send the email.",
                cancellationToken);
            return false;
        }

        try
        {
            SendEmail(client, email, server);
            await queuedEmailService.MarkAsSentAsync(email, server.User, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            StringBuilder reason = new(ex.Message);

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                _ = reason.Append('\n').Append(ex.Message);
            }

            await queuedEmailService.RecordSendFailureAsync(email.Id, reason.ToString(), cancellationToken);
            return false;
        }
    }

    private static void SendEmail(SmtpClient client, QueuedEmail email, MailServer server)
    {
        client.Host = server.Host;
        client.Port = server.Port;
        client.EnableSsl = server.EnableSSL;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(server.User, server.Password);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;

        using MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content
        };

        if (!string.IsNullOrWhiteSpace(server.FromEmail))
            message.From = new MailAddress(server.FromEmail);

        message.From ??= server.User.Contains('@')
            ? new MailAddress(server.User)
            : null;

        message.To.Add(email.To);
        client.Send(message);
    }
}
