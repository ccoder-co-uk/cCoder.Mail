// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal partial class SentEmailProcessingService(ISentEmailService service) : ISentEmailProcessingService
{
    public SentEmail GetSentEmail(int sentEmailId) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateGet(inputs: [sentEmailId]);

        return service.GetSentEmail(iSentEmailId: sentEmailId);
    });

    public IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAllSentEmail(ignoreFilters: ignoreFilters);
    });

    public ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newSentEmail]);

        return service.AddSentEmailAsync(newSentEmail: newSentEmail);
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedSentEmail]);

        return service.UpdateSentEmailAsync(updatedSentEmail: updatedSentEmail);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int sentEmailId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [sentEmailId]);

        return service.DeleteAsync(iSentEmailId: sentEmailId);
    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return service.DeleteAllByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdateSentEmailResult(IEnumerable<SentEmail> newSentEmail) =>
        TryCatch<IEnumerable<Result<SentEmail>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [newSentEmail]);

        List<Result<SentEmail>> results = new List<Result<SentEmail>>();

        foreach (SentEmail item in newSentEmail)
        {
            try
            {
                SentEmail savedItem =
                    item.Id == 0
                        ? await AddSentEmailAsync(newSentEmail: item)
                        : await UpdateSentEmailAsync(updatedSentEmail: item);

                results.Add(item: new Result<SentEmail>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<SentEmail>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }, isValueTask: true);

    public ValueTask DeleteAllSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [deletedSentEmail]);

        foreach (SentEmail item in deletedSentEmail)
        {
            await DeleteAsync(sentEmailId: item.Id);
        }
    }, isValueTask: true);
}