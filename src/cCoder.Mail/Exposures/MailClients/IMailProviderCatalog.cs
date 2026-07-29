// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures.MailClients;

public interface IMailProviderCatalog
{
    MailProviderSummary[] GetSenders();

    MailProviderSummary[] GetReceivers();
}