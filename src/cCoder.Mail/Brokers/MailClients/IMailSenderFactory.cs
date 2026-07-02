namespace cCoder.Mail.Brokers.MailClients;

public interface IMailSenderFactory
{
    IMailSenderProvider GetSender(string providerName);
}
