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
    public ValueTask AddAsync(App app) =>
        appOrchestrationService.AddAsync(app: app);
    public ValueTask UpdateAsync(App app) =>
        appOrchestrationService.UpdateAsync(app: app);
    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId: appId);
}