// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;

namespace cCoder.Mail.Exposures;

internal class MailAppExposure(IAppOrchestrationService appOrchestrationService) : IMailAppExposure
{
    public ValueTask AddAsync(App newApp) =>
        appOrchestrationService.AddAppAsync(newApp: newApp);

    public ValueTask UpdateAsync(App updatedApp) =>
        appOrchestrationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId: appId);
}