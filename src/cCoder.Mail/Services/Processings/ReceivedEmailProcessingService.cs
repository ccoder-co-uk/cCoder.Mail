// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class ReceivedEmailProcessingService(IReceivedEmailService service) : IReceivedEmailProcessingService
{
    public ReceivedEmail Get(int receivedEmailId) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateGet(inputs: [receivedEmailId]);

            return service.Get(iReceivedEmailId: receivedEmailId);
        });

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail newReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateAddAsync(inputs: [newReceivedEmail]);

            return service.AddAsync(newReceivedEmail: newReceivedEmail);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail updatedReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedReceivedEmail]);

            return service.UpdateAsync(updatedReceivedEmail: updatedReceivedEmail);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int receivedEmailId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [receivedEmailId]);

            return service.DeleteAsync(iReceivedEmailId: receivedEmailId);
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