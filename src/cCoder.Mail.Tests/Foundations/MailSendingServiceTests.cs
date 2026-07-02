using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    private readonly Mock<IMailClientBroker> mailClientBrokerMock;
    private readonly MailSendingService mailSendingService;

    public MailSendingServiceTests()
    {
        mailClientBrokerMock = new Mock<IMailClientBroker>(MockBehavior.Strict);
        mailSendingService = new MailSendingService(mailClientBrokerMock.Object);
    }
}
