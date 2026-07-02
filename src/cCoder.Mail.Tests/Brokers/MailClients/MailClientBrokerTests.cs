using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailClientBrokerTests
{
    private readonly Mock<IMailClient> mailClientMock;
    private readonly Mock<IMicrosoftGraphClient> microsoftGraphClientMock;
    private readonly MailClientBroker mailClientBroker;

    public MailClientBrokerTests()
    {
        mailClientMock = new Mock<IMailClient>(MockBehavior.Strict);
        microsoftGraphClientMock = new Mock<IMicrosoftGraphClient>(MockBehavior.Strict);
        mailClientBroker = new MailClientBroker(
            mailClientMock.Object,
            microsoftGraphClientMock.Object);
    }
}
