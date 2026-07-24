// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class QueuedEmailEventServiceTests
{
    private readonly Mock<IQueuedEmailEventBroker> queuedEmailEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.Mail.Services.Foundations.Events.QueuedEmailEventService service;
    private const string CurrentUserId = "test-user";

    public QueuedEmailEventServiceTests()
    {
        queuedEmailEventBrokerMock = new Mock<IQueuedEmailEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        queuedEmailEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock.Setup(expression: x => x.GetSsoUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Mail.Services.Foundations.Events.QueuedEmailEventService(
queuedEmailEventBroker: queuedEmailEventBrokerMock.Object,
authInfoBroker: authInfoMock.Object
        );
    }
}