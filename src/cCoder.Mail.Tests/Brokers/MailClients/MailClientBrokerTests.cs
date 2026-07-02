using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailClientBrokerTests
{
    private readonly Mock<IMailSenderFactory> mailSenderFactoryMock;
    private readonly Mock<IMailReceiverFactory> mailReceiverFactoryMock;
    private readonly Mock<IMailSenderProvider> mailSenderProviderMock;
    private readonly Mock<IMailReceiverProvider> mailReceiverProviderMock;
    private readonly MailClientBroker mailClientBroker;

    public MailClientBrokerTests()
    {
        mailSenderFactoryMock = new Mock<IMailSenderFactory>(MockBehavior.Strict);
        mailReceiverFactoryMock = new Mock<IMailReceiverFactory>(MockBehavior.Strict);
        mailSenderProviderMock = new Mock<IMailSenderProvider>(MockBehavior.Strict);
        mailReceiverProviderMock = new Mock<IMailReceiverProvider>(MockBehavior.Strict);
        mailClientBroker = new MailClientBroker(
            mailSenderFactoryMock.Object,
            mailReceiverFactoryMock.Object);
    }
}
