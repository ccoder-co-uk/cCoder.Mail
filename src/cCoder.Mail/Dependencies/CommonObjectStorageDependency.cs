// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Dependencies;

internal static class CommonObjectStorageDependency
{
    internal static CommonObject[] SelectLatestCommonObjectsPaged(
        ICoreContextFactory coreContextFactory,
        int pageSize)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        int skip = 0;
        CommonObject[] page;
        List<CommonObject> result = [];

        do
        {
            page = coreDataContext
                .CommonObjects
                .AsNoTracking()
                .GroupBy(keySelector: commonObject => new
                {
                    commonObject.Name,
                    commonObject.Culture,
                    commonObject.Key,
                    commonObject.Type,
                })
                .Select(selector: commonObjects => commonObjects
                    .OrderByDescending(
                        keySelector: commonObject => commonObject.Version)
                .First())
                .Skip(count: skip)
                .Take(count: pageSize)
                .ToArray();

            if (page.Length == 0)
            {
                break;
            }

            result.AddRange(collection: page);
            skip += pageSize;
        } while (true);

        return result.ToArray();
    }
}