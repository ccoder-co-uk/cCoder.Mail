using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class MailServerEventServiceTests
{
    private readonly Mock<IMailServerEventBroker> mailServerEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Mail.Services.Foundations.Events.MailServerEventService service;
    private const string CurrentUserId = "test-user";

    public MailServerEventServiceTests()
    {
        mailServerEventBrokerMock = new Mock<IMailServerEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        mailServerEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Mail.Services.Foundations.Events.MailServerEventService(
            mailServerEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









