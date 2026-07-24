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
    public QueuedEmail Get(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateGet(inputs: [queuedEmailId]);

        return service.Get(iQueuedEmailId: queuedEmailId);
    });

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail]);

        return AddAsync(newQueuedEmail: newQueuedEmail, checkPrivs: false);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail, checkPrivs]);

        return service.AddAsync(newQueuedEmail: newQueuedEmail, checkPrivileges: checkPrivs);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedQueuedEmail]);

        return service.UpdateAsync(updatedQueuedEmail: updatedQueuedEmail);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId]);

        QueuedEmail queuedEmail = GetAll(ignoreFilters: true)
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

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items) =>
        TryCatch<IEnumerable<Result<QueuedEmail>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        List<Result<QueuedEmail>> results = new List<Result<QueuedEmail>>();

        foreach (QueuedEmail item in items)
        {
            try
            {
                QueuedEmail savedItem =
                    item.Id == 0
                        ? await AddAsync(newQueuedEmail: item)
                        : await UpdateAsync(updatedQueuedEmail: item);

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

    public ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [items]);

        foreach (QueuedEmail item in items)
        {
            await DeleteAsync(queuedEmailId: item.Id);
        }
    }, isValueTask: true);
}