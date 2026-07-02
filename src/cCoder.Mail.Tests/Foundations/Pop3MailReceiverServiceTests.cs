using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class Pop3MailReceiverServiceTests
{
    private readonly Mock<IPop3MailReceiverBroker> pop3MailReceiverBrokerMock;
    private readonly Pop3MailReceiverService pop3MailReceiverService;

    public Pop3MailReceiverServiceTests()
    {
        pop3MailReceiverBrokerMock = new Mock<IPop3MailReceiverBroker>(MockBehavior.Strict);
        pop3MailReceiverService = new Pop3MailReceiverService(pop3MailReceiverBrokerMock.Object);
    }
}
