// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Mail.Dependencies;


namespace cCoder.Mail.Brokers.Storages;

public interface ICommonObjectBroker
{
    CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500);
}

internal sealed class CommonObjectBroker(ICoreContextFactory coreContextFactory) : ICommonObjectBroker
{
    public CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500) =>
        CommonObjectStorageDependency.SelectLatestCommonObjectsPaged(
            coreContextFactory: coreContextFactory,
            pageSize: pageSize);
}