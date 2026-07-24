// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class ReceivedEmailProcessingService(IReceivedEmailService service) : IReceivedEmailProcessingService
{
    public ReceivedEmail Get(int id) =>
        service.Get(id: id);

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) =>
        service.AddAsync(entity: entity);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) =>
        service.UpdateAsync(entity: entity);

    public ValueTask<int> DeleteAsync(int id) =>
        service.DeleteAsync(id: id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        service.AddRangeAsync(entities: entities, cancellationToken: cancellationToken);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        service.Exists(mailReceiverId: mailReceiverId, messageId: messageId);

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) =>
        service.DeleteAllAsync(items: items);
}