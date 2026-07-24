// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

internal sealed class MailConfigurationExposure(
    MailConfiguration mailConfiguration) : IMailConfigurationExposure
{
    public MailConfiguration GetMailConfiguration() =>
        mailConfiguration;
}