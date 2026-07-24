// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Exposures.MailClients;

public interface IMailReceiverFactory
{
    IMailReceiverProvider GetReceiver(string providerName);
}