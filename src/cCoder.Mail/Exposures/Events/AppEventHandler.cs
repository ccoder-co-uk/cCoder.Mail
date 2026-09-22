// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Mail.Services.Aggregations;

namespace cCoder.Mail.Exposures.Events;

internal sealed class AppEventHandler(
    IAppAggregationService appAggregationService)
    : IAppEventHandler
{
    public ValueTask AddAppAsync(App newApp) =>
        appAggregationService.AddAppAsync(newApp: newApp);

    public ValueTask UpdateAppAsync(App updatedApp) =>
        appAggregationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAppAsync(App deletedApp) =>
        appAggregationService.DeleteAsync(appId: deletedApp.Id);
}