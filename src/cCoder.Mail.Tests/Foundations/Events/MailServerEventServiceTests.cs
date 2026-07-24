// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        mailServerEventBrokerMock = new Mock<IMailServerEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(behavior: MockBehavior.Strict);
        mailServerEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: CurrentUserId);

        service = new cCoder.Mail.Services.Foundations.Events.MailServerEventService(
mailServerEventBroker: mailServerEventBrokerMock.Object,
authInfo: authInfoMock.Object
        );
    }
}