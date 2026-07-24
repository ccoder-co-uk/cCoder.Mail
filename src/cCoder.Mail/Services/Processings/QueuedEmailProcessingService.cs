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
    public QueuedEmail Get(int id) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateGet(inputs: [id]);

        return service.Get(id: id);
    });

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail entity) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [entity]);

        return AddAsync(email: entity, checkPrivs: false);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail email, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [email, checkPrivs]);

        return service.AddAsync(queuedEmail: email, checkPrivileges: checkPrivs);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        return service.UpdateAsync(queuedEmail: entity);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        QueuedEmail queuedEmail = GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: (QueuedEmail r) => r.Id == id);

        if (queuedEmail == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: "queuedemail_delete");
        await service.DeleteAsync(id: queuedEmail.Id, checkPrivileges: false);
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
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

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
            await DeleteAsync(id: item.Id);
        }
    }, isValueTask: true);
}