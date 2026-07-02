namespace cCoder.Mail.Exposures.MailClients;

public interface IMailSenderFactory
{
    IMailSenderProvider GetSender(string providerName);
}
