// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class SentEmailProcessingService(ISentEmailService service) : ISentEmailProcessingService
{
    public SentEmail Get(int id)
    {
        return service.Get(id: id);
    }

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<SentEmail> AddAsync(SentEmail entity)
    {
        return service.AddAsync(sentEmail: entity);
    }

    public ValueTask<SentEmail> UpdateAsync(SentEmail entity)
    {
        return service.UpdateAsync(sentEmail: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id: id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public async ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items)
    {
        List<Result<SentEmail>> results = new List<Result<SentEmail>>();

        foreach (SentEmail item in items)
        {
            try
            {
                SentEmail savedItem =
                    item.Id == 0
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

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
    }

    public async ValueTask DeleteAllAsync(IEnumerable<SentEmail> items)
    {
        foreach (SentEmail item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }
}