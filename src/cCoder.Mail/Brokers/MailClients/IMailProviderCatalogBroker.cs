// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Brokers.MailClients;

internal interface IMailProviderCatalogBroker
{
    IEnumerable<IMailClient> SelectAllMailClients();
}