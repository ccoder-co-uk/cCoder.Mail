using cCoder.Data;
using cCoder.Mail.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class QueuedEmailEventServiceTests
{
    private readonly Mock<IQueuedEmailEventBroker> queuedEmailEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Mail.Services.Foundations.Events.QueuedEmailEventService service;
    private const string CurrentUserId = "test-user";

    public QueuedEmailEventServiceTests()
    {
        queuedEmailEventBrokerMock = new Mock<IQueuedEmailEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        queuedEmailEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Mail.Services.Foundations.Events.QueuedEmailEventService(
            queuedEmailEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









