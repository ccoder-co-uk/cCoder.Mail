using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SmtpMailSenderServiceTests
{
    private readonly Mock<ISmtpMailSenderBroker> smtpMailSenderBrokerMock;
    private readonly SmtpMailSenderService smtpMailSenderService;

    public SmtpMailSenderServiceTests()
    {
        smtpMailSenderBrokerMock = new Mock<ISmtpMailSenderBroker>(MockBehavior.Strict);
        smtpMailSenderService = new SmtpMailSenderService(smtpMailSenderBrokerMock.Object);
    }
}
