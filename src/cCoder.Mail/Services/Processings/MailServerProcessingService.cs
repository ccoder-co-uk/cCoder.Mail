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
    public MailServer GetMailServer(int mailServerId) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [mailServerId]);

        return service.GetMailServer(iMailServerId: mailServerId);
    });

    public IQueryable<MailServer> GetAllMailServer(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAllMailServer(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateAddAsync(inputs: [newMailServer]);

        return service.AddMailServerAsync(newMailServer: newMailServer);
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailServer]);

        return service.UpdateMailServerAsync(updatedMailServer: updatedMailServer);
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

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdateMailServerResult(IEnumerable<MailServer> newMailServer) =>
        TryCatch<IEnumerable<Result<MailServer>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [newMailServer]);

        List<Result<MailServer>> results = new List<Result<MailServer>>();

        foreach (MailServer item in newMailServer)
        {
            try
            {
                MailServer savedItem =
                    item.Id == 0
                        ? await AddMailServerAsync(newMailServer: item)
                        : await UpdateMailServerAsync(updatedMailServer: item);

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

    public ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> deletedMailServer) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [deletedMailServer]);

        foreach (MailServer item in deletedMailServer)
        {
            await DeleteAsync(mailServerId: item.Id);
        }
    }, isValueTask: true);
}