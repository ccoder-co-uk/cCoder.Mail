using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailClientBroker(
    IMailClient mailClient,
    IMicrosoftGraphClient microsoftGraphClient) : IMailClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        ShouldUseMicrosoftGraph(email)
            ? microsoftGraphClient.SendAsync(email, cancellationToken)
            : mailClient.SendAsync(email, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        microsoftGraphClient.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        microsoftGraphClient.ReceiveTopAsync(count, cancellationToken);

    private static bool ShouldUseMicrosoftGraph(QueuedEmail email)
    {
        MailServer server = email?.App?.MailServers?.FirstOrDefault(
            mailServer => mailServer.Name == email.MailServerName);

        return string.Equals(server?.Host, "graph.microsoft.com", StringComparison.OrdinalIgnoreCase)
            || string.Equals(server?.Host, "microsoft-graph", StringComparison.OrdinalIgnoreCase)
            || string.Equals(server?.Host, "https://graph.microsoft.com", StringComparison.OrdinalIgnoreCase);
    }
}
