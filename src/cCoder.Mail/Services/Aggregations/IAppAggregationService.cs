// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Aggregations;

public interface IAppAggregationService
{
    ValueTask AddAppAsync(App newApp);
    ValueTask UpdateAppAsync(App updatedApp);
    ValueTask DeleteAsync(int appId);
}