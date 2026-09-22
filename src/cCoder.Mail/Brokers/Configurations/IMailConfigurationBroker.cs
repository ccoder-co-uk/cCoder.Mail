// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.Configurations;

internal interface IMailConfigurationBroker
{
    MailConfiguration GetMailConfiguration();
}