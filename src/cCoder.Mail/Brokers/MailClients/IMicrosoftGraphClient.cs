using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMicrosoftGraphClient
{
    Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}
