// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

public interface IMailProviderCatalogService
{
    MailProviderSummary[] GetSenders();

    MailProviderSummary[] GetReceivers();
}