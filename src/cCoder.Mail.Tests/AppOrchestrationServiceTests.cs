// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests;

public class AppOrchestrationServiceTests
{
    private readonly Mock<IMailServerOrchestrationService> mailServerOrchestrationServiceMock;
    private readonly Mock<IMailSenderConfigurationOrchestrationService> mailSenderConfigurationOrchestrationServiceMock;
    private readonly Mock<IMailReceiverConfigurationOrchestrationService> mailReceiverConfigurationOrchestrationServiceMock;
    private readonly Mock<IQueuedEmailOrchestrationService> queuedEmailOrchestrationServiceMock;
    private readonly Mock<ISentEmailOrchestrationService> sentEmailOrchestrationServiceMock;
    private readonly Mock<IReceivedEmailOrchestrationService> receivedEmailOrchestrationServiceMock;
    private readonly AppOrchestrationService service;

    public AppOrchestrationServiceTests()
    {
        mailServerOrchestrationServiceMock = new Mock<IMailServerOrchestrationService>(behavior: MockBehavior.Strict);
        mailSenderConfigurationOrchestrationServiceMock = new Mock<IMailSenderConfigurationOrchestrationService>(behavior: MockBehavior.Strict);
        mailReceiverConfigurationOrchestrationServiceMock = new Mock<IMailReceiverConfigurationOrchestrationService>(behavior: MockBehavior.Strict);
        queuedEmailOrchestrationServiceMock = new Mock<IQueuedEmailOrchestrationService>(behavior: MockBehavior.Strict);
        sentEmailOrchestrationServiceMock = new Mock<ISentEmailOrchestrationService>(behavior: MockBehavior.Strict);
        receivedEmailOrchestrationServiceMock = new Mock<IReceivedEmailOrchestrationService>(behavior: MockBehavior.Strict);

        service = new AppOrchestrationService(
mailServerOrchestrationService: mailServerOrchestrationServiceMock.Object,
mailSenderConfigurationOrchestrationService: mailSenderConfigurationOrchestrationServiceMock.Object,
mailReceiverConfigurationOrchestrationService: mailReceiverConfigurationOrchestrationServiceMock.Object,
queuedEmailOrchestrationService: queuedEmailOrchestrationServiceMock.Object,
sentEmailOrchestrationService: sentEmailOrchestrationServiceMock.Object,
receivedEmailOrchestrationService: receivedEmailOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedMailRowsByAppIdWhenDeleteAsync()
    {
        mailServerOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        mailSenderConfigurationOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        mailReceiverConfigurationOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        queuedEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        sentEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        receivedEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        await service.DeleteAsync(appId: 5);

        mailServerOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailSenderConfigurationOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailReceiverConfigurationOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        queuedEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        sentEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        receivedEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailServerOrchestrationServiceMock.VerifyNoOtherCalls();
        mailSenderConfigurationOrchestrationServiceMock.VerifyNoOtherCalls();
        mailReceiverConfigurationOrchestrationServiceMock.VerifyNoOtherCalls();
        queuedEmailOrchestrationServiceMock.VerifyNoOtherCalls();
        sentEmailOrchestrationServiceMock.VerifyNoOtherCalls();
        receivedEmailOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldStampMailAppIdsWhenAddAsync()
    {
        App app = new()
        {
            Id = 9,
            MailServers = [new MailServer { Id = 1, Name = "Server" }],
            MailQueue = [new QueuedEmail { Id = 2, Subject = "Queued" }],
            SentMail = [new SentEmail { Id = 3, Subject = "Sent" }]
        };

        mailServerOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdate(
items: It.Is<IEnumerable<MailServer>>(match: items => items.All(predicate: server => server.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<MailServer>>>(result: []));

        queuedEmailOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdate(
items: It.Is<IEnumerable<QueuedEmail>>(match: items => items.All(predicate: email => email.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<QueuedEmail>>>(result: []));

        sentEmailOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdate(
items: It.Is<IEnumerable<SentEmail>>(match: items => items.All(predicate: email => email.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<SentEmail>>>(result: []));

        await service.AddAsync(app: app);

        mailServerOrchestrationServiceMock.VerifyAll();
        queuedEmailOrchestrationServiceMock.VerifyAll();
        sentEmailOrchestrationServiceMock.VerifyAll();
    }
}