using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailSenderClientBrokerTests
{
    private readonly Mock<IMailSenderFactory> mailSenderFactoryMock;
    private readonly Mock<IMailSenderProvider> mailSenderProviderMock;
    private readonly MailSenderClientBroker mailSenderClientBroker;

    public MailSenderClientBrokerTests()
    {
        mailSenderFactoryMock = new Mock<IMailSenderFactory>(MockBehavior.Strict);
        mailSenderProviderMock = new Mock<IMailSenderProvider>(MockBehavior.Strict);
        mailSenderClientBroker = new MailSenderClientBroker(mailSenderFactoryMock.Object);
    }
}
