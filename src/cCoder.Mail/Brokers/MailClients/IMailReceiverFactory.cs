namespace cCoder.Mail.Brokers.MailClients;

public interface IMailReceiverFactory
{
    IMailReceiverProvider GetReceiver(string providerName);
}
