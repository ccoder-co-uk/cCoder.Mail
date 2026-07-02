using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceivingServiceTests
{
    private readonly Mock<IMailReceiverClientBroker> mailReceiverClientBrokerMock;
    private readonly MailReceivingService mailReceivingService;

    public MailReceivingServiceTests()
    {
        mailReceiverClientBrokerMock = new Mock<IMailReceiverClientBroker>(MockBehavior.Strict);
        mailReceivingService = new MailReceivingService(mailReceiverClientBrokerMock.Object);
    }
}
