// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class ReceivedEmailProcessingService(IReceivedEmailService service) : IReceivedEmailProcessingService
{
    public ReceivedEmail GetReceivedEmail(int receivedEmailId) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateGet(inputs: [receivedEmailId]);

            return service.GetReceivedEmail(iReceivedEmailId: receivedEmailId);
        });

    public IQueryable<ReceivedEmail> GetAllReceivedEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return service.GetAllReceivedEmail(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddReceivedEmailAsync(ReceivedEmail newReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateAddAsync(inputs: [newReceivedEmail]);

            return service.AddReceivedEmailAsync(newReceivedEmail: newReceivedEmail);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(ReceivedEmail updatedReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedReceivedEmail]);

            return service.UpdateReceivedEmailAsync(updatedReceivedEmail: updatedReceivedEmail);
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

    public ValueTask AddRangeReceivedEmailAsync(
        IEnumerable<ReceivedEmail> entities,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateAddRangeAsync(inputs: [entities, cancellationToken]);

            return service.AddRangeReceivedEmailAsync(entities: entities, cancellationToken: cancellationToken);
        }, isValueTask: true);

    public bool Exists(Guid mailReceiverId, string messageId) =>
        TryCatch<bool>(operation: () =>
        {
            ValidateExists(inputs: [mailReceiverId, messageId]);

            return service.Exists(mailReceiverId: mailReceiverId, messageId: messageId);
        });

    public ValueTask DeleteAllReceivedEmailAsync(IEnumerable<ReceivedEmail> items) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAllAsync(inputs: [items]);

            return service.DeleteAllReceivedEmailAsync(items: items);
        }, isValueTask: true);
}