// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMailProviderService
{
    Exposures.MailClients.IMailClient GetMailClient(
        string providerName);

}