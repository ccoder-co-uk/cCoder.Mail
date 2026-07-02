using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    private readonly Mock<IMailSenderClientBroker> mailSenderClientBrokerMock;
    private readonly MailSendingService mailSendingService;

    public MailSendingServiceTests()
    {
        mailSenderClientBrokerMock = new Mock<IMailSenderClientBroker>(MockBehavior.Strict);
        mailSendingService = new MailSendingService(mailSenderClientBrokerMock.Object);
    }
}
