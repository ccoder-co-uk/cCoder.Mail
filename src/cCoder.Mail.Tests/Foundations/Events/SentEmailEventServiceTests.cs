// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class SentEmailEventServiceTests
{
    private readonly Mock<ISentEmailEventBroker> sentEmailEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.Mail.Services.Foundations.Events.SentEmailEventService service;
    private const string CurrentUserId = "test-user";

    public SentEmailEventServiceTests()
    {
        sentEmailEventBrokerMock = new Mock<ISentEmailEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        sentEmailEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock.Setup(expression: x => x.GetSsoUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Mail.Services.Foundations.Events.SentEmailEventService(
sentEmailEventBroker: sentEmailEventBrokerMock.Object,
authInfoBroker: authInfoMock.Object
        );
    }
}