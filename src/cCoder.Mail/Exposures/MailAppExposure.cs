// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Aggregations;

namespace cCoder.Mail.Exposures;

internal class MailAppExposure(IAppAggregationService appAggregationService) : IMailAppExposure
{
    public ValueTask AddAsync(App newApp) =>
        appAggregationService.AddAppAsync(newApp: newApp);

    public ValueTask UpdateAsync(App updatedApp) =>
        appAggregationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAsync(int appId) =>
        appAggregationService.DeleteAsync(appId: appId);
}