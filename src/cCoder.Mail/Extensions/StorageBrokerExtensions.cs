// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Extensions;

internal static class StorageBrokerExtensions
{
    internal static IQueryable<T> SelectAll<T>(
        DbSet<T> entities,
        bool ignoreFilters)
        where T : class =>
        ignoreFilters
                ? entities.IgnoreQueryFilters()
                : entities;

    internal static T[] Normalize<T>(IEnumerable<T> entities) =>
        entities?.ToArray() ?? [];
}