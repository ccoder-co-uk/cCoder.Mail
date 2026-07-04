using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface IQueuedEmailService
{
    QueuedEmail Get(int id);
    IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false);
    QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures);
    ValueTask<QueuedEmail> AddAsync(QueuedEmail queuedEmail, bool checkPrivileges = true);
    ValueTask<QueuedEmail> UpdateAsync(QueuedEmail queuedEmail);
    ValueTask RecordSendFailureAsync(int emailId, string reason, CancellationToken cancellationToken = default);
    ValueTask MarkAsSentAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(int id, bool checkPrivileges = true);
    ValueTask DeleteAllForAppAsync(IEnumerable<QueuedEmail> items);
}









