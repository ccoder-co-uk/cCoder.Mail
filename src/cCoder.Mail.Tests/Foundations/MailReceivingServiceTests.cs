using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceivingServiceTests
{
    private readonly Mock<IMailClientBroker> mailClientBrokerMock;
    private readonly MailReceivingService mailReceivingService;

    public MailReceivingServiceTests()
    {
        mailClientBrokerMock = new Mock<IMailClientBroker>(MockBehavior.Strict);
        mailReceivingService = new MailReceivingService(mailClientBrokerMock.Object);
    }
}
