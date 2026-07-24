// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class MailServerProcessingService(IMailServerService service) : IMailServerProcessingService
{
    public MailServer Get(int id) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [id]);

        return service.Get(id: id);
    });

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddAsync(MailServer entity) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateAddAsync(inputs: [entity]);

        return service.AddAsync(mailServer: entity);
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer entity) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        return service.UpdateAsync(mailServer: entity);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: () =>
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

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items) =>
        TryCatch<IEnumerable<Result<MailServer>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        List<Result<MailServer>> results = new List<Result<MailServer>>();

        foreach (MailServer item in items)
        {
            try
            {
                MailServer savedItem =
                    item.Id == 0
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<MailServer>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<MailServer>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<MailServer> items) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [items]);

        foreach (MailServer item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }, isValueTask: true);
}