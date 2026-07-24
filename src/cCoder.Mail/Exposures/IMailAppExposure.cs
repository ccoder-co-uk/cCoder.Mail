// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Exposures;

public interface IMailAppExposure
{
    ValueTask AddAsync(App newApp);
    ValueTask UpdateAsync(App updatedApp);
    ValueTask DeleteAsync(int appId);
}