using cCoder.Data;
using cCoder.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Mail.Brokers.Storages;

public interface ICommonObjectBroker
{
    CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500);
}

public class CommonObjectBroker(ICoreContextFactory coreContextFactory) : ICommonObjectBroker
{
    public CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        int skip = 0;
        CommonObject[] page;
        List<CommonObject> result = [];

        do
        {
            page = coreDataContext
                .CommonObjects
                .AsNoTracking()
                .GroupBy(c => new
                {
                    c.Name,
                    c.Culture,
                    c.Key,
                    c.Type,
                })
                .Select(c => c.OrderByDescending(v => v.Version).First())
                .Skip(skip)
                .Take(pageSize)
                .ToArray();

            if (page.Length == 0)
                break;

            result.AddRange(page);
            skip += pageSize;
        } while (true);

        return result.ToArray();
    }
}


