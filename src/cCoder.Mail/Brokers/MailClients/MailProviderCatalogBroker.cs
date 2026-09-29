// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailProviderCatalogBroker(
    IEnumerable<IMailClient> mailClients)
    : IMailProviderCatalogBroker
{
    public IEnumerable<IMailClient> SelectAllMailClients() =>
        mailClients;
}