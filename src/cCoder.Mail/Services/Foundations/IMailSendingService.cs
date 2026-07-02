using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMailSendingService
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}
