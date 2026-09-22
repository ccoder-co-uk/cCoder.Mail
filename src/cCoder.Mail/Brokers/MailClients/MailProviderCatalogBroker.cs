// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailProviderCatalogBroker(
    IEnumerable<IMailClient> mailClients)
    : IMailProviderCatalogBroker
{
    public IEnumerable<IMailClient> SelectAllMailClients() =>
        mailClients;
}