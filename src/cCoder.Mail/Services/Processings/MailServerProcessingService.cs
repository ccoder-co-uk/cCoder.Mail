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
    public MailServer Get(int mailServerId) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [mailServerId]);

        return service.Get(iMailServerId: mailServerId);
    });

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateAddAsync(inputs: [newMailServer]);

        return service.AddAsync(newMailServer: newMailServer);
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailServer]);

        return service.UpdateAsync(updatedMailServer: updatedMailServer);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int mailServerId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [mailServerId]);

        return service.DeleteAsync(iMailServerId: mailServerId);
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
                        ? await AddAsync(newMailServer: item)
                        : await UpdateAsync(updatedMailServer: item);

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
            await DeleteAsync(mailServerId: item.Id);
        }
    }, isValueTask: true);
}