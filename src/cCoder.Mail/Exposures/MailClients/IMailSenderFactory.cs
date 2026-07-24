// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Exposures.MailClients;

public interface IMailSenderFactory
{
    IMailSenderProvider GetSender(string providerName);
}