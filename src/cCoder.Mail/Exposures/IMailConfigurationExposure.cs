// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

public interface IMailConfigurationExposure
{
    MailConfiguration GetMailConfiguration();
}