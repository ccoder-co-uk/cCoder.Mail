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

internal class QueuedEmailProcessingService(IQueuedEmailService service, IAuthorizationBroker authorizationBroker) : IQueuedEmailProcessingService
{
    public QueuedEmail Get(int id)
    {
        return service.Get(id: id);
    }

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail entity)
    {
        return AddAsync(email: entity, checkPrivs: false);
    }

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail email, bool checkPrivs)
    {
        return service.AddAsync(queuedEmail: email, checkPrivileges: checkPrivs);
    }

    public ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity)
    {
        return service.UpdateAsync(queuedEmail: entity);
    }

    public async ValueTask DeleteAsync(int id)
    {
        QueuedEmail queuedEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (QueuedEmail r) => r.Id == id);

        if (queuedEmail == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        authorizationBroker.Authorize(appId: queuedEmail.AppId, privilege: "queuedemail_delete");
        await service.DeleteAsync(id: queuedEmail.Id, checkPrivileges: false);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public async ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items)
    {
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
    }

    public async ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items)
    {
        foreach (QueuedEmail item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }
}