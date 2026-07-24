// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Orchestrations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailClientOrchestrationServiceTests
{
    private readonly Mock<IMailSendingService> mailSendingServiceMock;
    private readonly Mock<IMailReceivingService> mailReceivingServiceMock;
    private readonly MailClientOrchestrationService mailClientOrchestrationService;

    public MailClientOrchestrationServiceTests()
    {
        mailSendingServiceMock = new Mock<IMailSendingService>(behavior: MockBehavior.Strict);
        mailReceivingServiceMock = new Mock<IMailReceivingService>(behavior: MockBehavior.Strict);

        mailClientOrchestrationService = new MailClientOrchestrationService(
mailSendingService: mailSendingServiceMock.Object,
mailReceivingService: mailReceivingServiceMock.Object);
    }
}