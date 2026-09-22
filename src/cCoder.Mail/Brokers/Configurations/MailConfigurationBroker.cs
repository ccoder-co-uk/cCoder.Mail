// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Mail.Brokers.Configurations;

internal sealed class MailConfigurationBroker(
    MailConfiguration mailConfiguration)
    : IMailConfigurationBroker, IUtilityBroker
{
    public MailConfiguration GetMailConfiguration() =>
        mailConfiguration;
}