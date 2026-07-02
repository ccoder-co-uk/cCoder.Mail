using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailReceiverClientBrokerTests
{
    private readonly Mock<IMailReceiverFactory> mailReceiverFactoryMock;
    private readonly Mock<IMailReceiverProvider> mailReceiverProviderMock;
    private readonly MailReceiverClientBroker mailReceiverClientBroker;

    public MailReceiverClientBrokerTests()
    {
        mailReceiverFactoryMock = new Mock<IMailReceiverFactory>(MockBehavior.Strict);
        mailReceiverProviderMock = new Mock<IMailReceiverProvider>(MockBehavior.Strict);
        mailReceiverClientBroker = new MailReceiverClientBroker(mailReceiverFactoryMock.Object);
    }
}
