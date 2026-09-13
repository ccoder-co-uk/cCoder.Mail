// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal interface IMailClientRegistryBroker
{
    IMailClient SelectMailClient(string providerName);
}