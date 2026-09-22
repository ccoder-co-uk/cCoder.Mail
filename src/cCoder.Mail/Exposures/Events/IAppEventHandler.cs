// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.Mail.Exposures.Events;

public interface IAppEventHandler
{
    ValueTask AddAppAsync(App newApp);

    ValueTask UpdateAppAsync(App updatedApp);

    ValueTask DeleteAppAsync(App deletedApp);
}