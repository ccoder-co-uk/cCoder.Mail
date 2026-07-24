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
    public SentEmail Get(int sentEmailId) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateGet(inputs: [sentEmailId]);

        return service.Get(iSentEmailId: sentEmailId);
    });

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return service.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<SentEmail> AddAsync(SentEmail newSentEmail) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateAddAsync(inputs: [newSentEmail]);

        return service.AddAsync(newSentEmail: newSentEmail);
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateAsync(SentEmail updatedSentEmail) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateUpdateAsync(inputs: [updatedSentEmail]);

        return service.UpdateAsync(updatedSentEmail: updatedSentEmail);
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

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items) =>
        TryCatch<IEnumerable<Result<SentEmail>>>(operation: async () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        List<Result<SentEmail>> results = new List<Result<SentEmail>>();

        foreach (SentEmail item in items)
        {
            try
            {
                SentEmail savedItem =
                    item.Id == 0
                        ? await AddAsync(newSentEmail: item)
                        : await UpdateAsync(updatedSentEmail: item);

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

    public ValueTask DeleteAllAsync(IEnumerable<SentEmail> items) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAllAsync(inputs: [items]);

        foreach (SentEmail item in items)
        {
            await DeleteAsync(sentEmailId: item.Id);
        }
    }, isValueTask: true);
}