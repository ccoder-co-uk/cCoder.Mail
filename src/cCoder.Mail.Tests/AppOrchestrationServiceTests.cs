// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Aggregations;
using cCoder.Mail.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests;

public class AppAggregationServiceTests
{
    private readonly Mock<IMailServerOrchestrationService> mailServerOrchestrationServiceMock;
    private readonly Mock<IMailSenderOrchestrationService> mailSenderOrchestrationServiceMock;
    private readonly Mock<IMailReceiverOrchestrationService> mailReceiverOrchestrationServiceMock;
    private readonly Mock<IQueuedEmailOrchestrationService> queuedEmailOrchestrationServiceMock;
    private readonly Mock<ISentEmailOrchestrationService> sentEmailOrchestrationServiceMock;
    private readonly Mock<IReceivedEmailOrchestrationService> receivedEmailOrchestrationServiceMock;
    private readonly AppAggregationService service;

    public AppAggregationServiceTests()
    {
        mailServerOrchestrationServiceMock = new Mock<IMailServerOrchestrationService>(behavior: MockBehavior.Strict);
        mailSenderOrchestrationServiceMock = new Mock<IMailSenderOrchestrationService>(behavior: MockBehavior.Strict);
        mailReceiverOrchestrationServiceMock = new Mock<IMailReceiverOrchestrationService>(behavior: MockBehavior.Strict);
        queuedEmailOrchestrationServiceMock = new Mock<IQueuedEmailOrchestrationService>(behavior: MockBehavior.Strict);
        sentEmailOrchestrationServiceMock = new Mock<ISentEmailOrchestrationService>(behavior: MockBehavior.Strict);
        receivedEmailOrchestrationServiceMock = new Mock<IReceivedEmailOrchestrationService>(behavior: MockBehavior.Strict);

        service = new AppAggregationService(
mailServerOrchestrationService: mailServerOrchestrationServiceMock.Object,
mailSenderOrchestrationService: mailSenderOrchestrationServiceMock.Object,
mailReceiverOrchestrationService: mailReceiverOrchestrationServiceMock.Object,
queuedEmailOrchestrationService: queuedEmailOrchestrationServiceMock.Object,
sentEmailOrchestrationService: sentEmailOrchestrationServiceMock.Object,
receivedEmailOrchestrationService: receivedEmailOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedMailRowsByAppIdWhenDeleteAsync()
    {
        mailServerOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        mailSenderOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        mailReceiverOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        queuedEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        sentEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        receivedEmailOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        await service.DeleteAsync(appId: 5);

        mailServerOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailSenderOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailReceiverOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        queuedEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        sentEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        receivedEmailOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        mailServerOrchestrationServiceMock.VerifyNoOtherCalls();
        mailSenderOrchestrationServiceMock.VerifyNoOtherCalls();
        mailReceiverOrchestrationServiceMock.VerifyNoOtherCalls();
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

        mailServerOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateMailServerResult(
newMailServer: It.Is<IEnumerable<MailServer>>(match: items => items.All(predicate: server => server.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<MailServer>>>(result: []));

        queuedEmailOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateQueuedEmailResult(
newQueuedEmail: It.Is<IEnumerable<QueuedEmail>>(match: items => items.All(predicate: email => email.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<QueuedEmail>>>(result: []));

        sentEmailOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateSentEmailResult(
newSentEmail: It.Is<IEnumerable<SentEmail>>(match: items => items.All(predicate: email => email.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<SentEmail>>>(result: []));

        await service.AddAppAsync(newApp: app);

        mailServerOrchestrationServiceMock.VerifyAll();
        queuedEmailOrchestrationServiceMock.VerifyAll();
        sentEmailOrchestrationServiceMock.VerifyAll();
    }
}
