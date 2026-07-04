using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class MailServerProcessingService(IMailServerService service) : IMailServerProcessingService
{
    public MailServer Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<MailServer> AddAsync(MailServer entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<MailServer> UpdateAsync(MailServer entity)
    {
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id);
    }

    public ValueTask DeleteByAppIdAsync(int appId)
    {
        return service.DeleteAllForAppAsync(
            GetAll(ignoreFilters: true)
                .Where(item => item.AppId == appId)
                .ToArray());
    }

    public async ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items)
    {
        List<Result<MailServer>> results = new List<Result<MailServer>>();

        foreach (MailServer item in items)
        {
            try
            {
                MailServer savedItem =
                    item.Id == 0
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<MailServer>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<MailServer>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<MailServer> items)
    {
        foreach (MailServer item in items)
        {
            await DeleteAsync(item.Id);
        }
    }
}
