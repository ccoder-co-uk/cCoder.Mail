// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Mail.Brokers;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class QueuedEmailProcessingService(IQueuedEmailService service, IAuthorizationBroker authorizationBroker) : IQueuedEmailProcessingService
{
    public QueuedEmail GetQueuedEmail(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateGet(inputs: [queuedEmailId]);

        return service.GetQueuedEmail(iQueuedEmailId: queuedEmailId);
    });

    public IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAllQueuedEmail(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail]);

        return AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail, checkPrivs: false);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail, checkPrivs]);

        return service.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail, checkPrivileges: checkPrivs);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedQueuedEmail]);

        return service.UpdateQueuedEmailAsync(updatedQueuedEmail: updatedQueuedEmail);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId]);

        QueuedEmail queuedEmail = GetAllQueuedEmail(ignoreFilters: true)
            .FirstOrDefault(predicate: (QueuedEmail r) => r.Id == queuedEmailId);

        if (queuedEmail == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: "queuedemail_delete");
        await service.DeleteAsync(iQueuedEmailId: queuedEmail.Id, checkPrivileges: false);
    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdateQueuedEmailResult(IEnumerable<QueuedEmail> newQueuedEmail) =>
        TryCatch<IEnumerable<Result<QueuedEmail>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [newQueuedEmail]);

        List<Result<QueuedEmail>> results = new List<Result<QueuedEmail>>();

        foreach (QueuedEmail item in newQueuedEmail)
        {
            try
            {
                QueuedEmail savedItem =
                    item.Id == 0
                        ? await AddQueuedEmailAsync(newQueuedEmail: item)
                        : await UpdateQueuedEmailAsync(updatedQueuedEmail: item);

                results.Add(item: new Result<QueuedEmail>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<QueuedEmail>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }, isValueTask: true);

    public ValueTask DeleteAllQueuedEmailAsync(IEnumerable<QueuedEmail> deletedQueuedEmail) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [deletedQueuedEmail]);

        foreach (QueuedEmail item in deletedQueuedEmail)
        {
            await DeleteAsync(queuedEmailId: item.Id);
        }
    }, isValueTask: true);
}