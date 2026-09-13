// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal interface IMailProviderCatalogService
{
    MailProviderSummary[] GetSenders();

    MailProviderSummary[] GetReceivers();
}