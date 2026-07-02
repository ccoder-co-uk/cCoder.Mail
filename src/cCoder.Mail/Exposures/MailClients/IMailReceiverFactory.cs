namespace cCoder.Mail.Exposures.MailClients;

public interface IMailReceiverFactory
{
    IMailReceiverProvider GetReceiver(string providerName);
}
