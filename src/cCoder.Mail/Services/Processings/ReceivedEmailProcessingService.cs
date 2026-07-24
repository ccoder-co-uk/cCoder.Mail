// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class ReceivedEmailProcessingService(IReceivedEmailService service) : IReceivedEmailProcessingService
{
    public ReceivedEmail Get(int id) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return service.Get(id: id);
        });

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return service.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [entity]);

            return service.UpdateAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int id) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [id]);

            return service.DeleteAsync(id: id);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask AddRangeAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateAddRangeAsync(inputs: [entities, cancellationToken]);

            return service.AddRangeAsync(entities: entities, cancellationToken: cancellationToken);
        }, isValueTask: true);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        TryCatch<bool>(operation: () =>
        {
            ValidateExists(inputs: [mailReceiverId, messageId]);

            return service.Exists(mailReceiverId: mailReceiverId, messageId: messageId);
        });

    public ValueTask DeleteAllAsync(IEnumerable<ReceivedEmail> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return service.DeleteAllAsync(items: items);
        }, isValueTask: true);
}