using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class SentEmailEventServiceTests
{
    private readonly Mock<ISentEmailEventBroker> sentEmailEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Mail.Services.Foundations.Events.SentEmailEventService service;
    private const string CurrentUserId = "test-user";

    public SentEmailEventServiceTests()
    {
        sentEmailEventBrokerMock = new Mock<ISentEmailEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        sentEmailEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Mail.Services.Foundations.Events.SentEmailEventService(
            sentEmailEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









