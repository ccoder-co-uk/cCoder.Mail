using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class ReceivedEmailProcessingService(IReceivedEmailService service) : IReceivedEmailProcessingService
{
    public ReceivedEmail Get(int id) => service.Get(id);

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters);

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) => service.AddAsync(entity);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) => service.UpdateAsync(entity);

    public ValueTask<int> DeleteAsync(int id) => service.DeleteAsync(id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllAsync(
            GetAll(ignoreFilters: true)
                .Where(item => item.AppId == appId)
                .ToArray());

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        service.AddRangeAsync(entities, cancellationToken);

    public bool Exists(Guid mailReceiverId, string messageId) => service.Exists(mailReceiverId, messageId);

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) => service.DeleteAllAsync(items);
}
