using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IReceivedEmailService
{
    ReceivedEmail Get(int id);
    IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false);
    ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity);
    ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity);
    ValueTask<int> DeleteAsync(int id);
    ValueTask AddRangeAsync(IEnumerable<ReceivedEmail> entities, CancellationToken cancellationToken = default);
    bool Exists(Guid mailReceiverId, string messageId);
    ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}
