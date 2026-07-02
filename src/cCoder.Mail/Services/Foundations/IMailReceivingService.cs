using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

public interface IMailReceivingService
{
    Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}
