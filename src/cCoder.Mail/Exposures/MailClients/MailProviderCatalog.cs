// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class MailProviderCatalog(
    IMailProviderCatalogService mailProviderCatalogService)
    : IMailProviderCatalog
{
    public MailProviderSummary[] GetSenders() =>
        mailProviderCatalogService.GetSenders();

    public MailProviderSummary[] GetReceivers() =>
        mailProviderCatalogService.GetReceivers();
}