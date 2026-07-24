// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        queuedEmailEventBrokerMock = new Mock<IQueuedEmailEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(behavior: MockBehavior.Strict);
        queuedEmailEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: CurrentUserId);

        service = new cCoder.Mail.Services.Foundations.Events.QueuedEmailEventService(
queuedEmailEventBroker: queuedEmailEventBrokerMock.Object,
authInfo: authInfoMock.Object
        );
    }
}