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
        mailServerOrchestrationServiceMock = new Mock<IMailServerOrchestrationService>(MockBehavior.Strict);
        mailSenderConfigurationOrchestrationServiceMock = new Mock<IMailSenderConfigurationOrchestrationService>(MockBehavior.Strict);
        mailReceiverConfigurationOrchestrationServiceMock = new Mock<IMailReceiverConfigurationOrchestrationService>(MockBehavior.Strict);
        queuedEmailOrchestrationServiceMock = new Mock<IQueuedEmailOrchestrationService>(MockBehavior.Strict);
        sentEmailOrchestrationServiceMock = new Mock<ISentEmailOrchestrationService>(MockBehavior.Strict);
        receivedEmailOrchestrationServiceMock = new Mock<IReceivedEmailOrchestrationService>(MockBehavior.Strict);
        service = new AppOrchestrationService(
            mailServerOrchestrationServiceMock.Object,
            mailSenderConfigurationOrchestrationServiceMock.Object,
            mailReceiverConfigurationOrchestrationServiceMock.Object,
            queuedEmailOrchestrationServiceMock.Object,
            sentEmailOrchestrationServiceMock.Object,
            receivedEmailOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedMailRowsByAppIdWhenDeleteAsync()
    {
        mailServerOrchestrationServiceMock.Setup(x => x.DeleteByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);
        mailSenderConfigurationOrchestrationServiceMock.Setup(x => x.GetAll(true))
            .Returns(Array.Empty<MailSender>().AsQueryable());
        mailReceiverConfigurationOrchestrationServiceMock.Setup(x => x.GetAll(true))
            .Returns(Array.Empty<MailReceiver>().AsQueryable());
        queuedEmailOrchestrationServiceMock.Setup(x => x.DeleteByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);
        sentEmailOrchestrationServiceMock.Setup(x => x.DeleteByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);
        receivedEmailOrchestrationServiceMock.Setup(x => x.GetAll(true))
            .Returns(Array.Empty<ReceivedEmail>().AsQueryable());

        await service.DeleteAsync(5);

        mailServerOrchestrationServiceMock.Verify(x => x.DeleteByAppIdAsync(5), Times.Once);
        mailSenderConfigurationOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        mailReceiverConfigurationOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        queuedEmailOrchestrationServiceMock.Verify(x => x.DeleteByAppIdAsync(5), Times.Once);
        sentEmailOrchestrationServiceMock.Verify(x => x.DeleteByAppIdAsync(5), Times.Once);
        receivedEmailOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
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

        mailServerOrchestrationServiceMock.Setup(x => x.AddOrUpdate(
                It.Is<IEnumerable<MailServer>>(items => items.All(server => server.AppId == 9))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<MailServer>>>([]));
        queuedEmailOrchestrationServiceMock.Setup(x => x.AddOrUpdate(
                It.Is<IEnumerable<QueuedEmail>>(items => items.All(email => email.AppId == 9))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<QueuedEmail>>>([]));
        sentEmailOrchestrationServiceMock.Setup(x => x.AddOrUpdate(
                It.Is<IEnumerable<SentEmail>>(items => items.All(email => email.AppId == 9))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.Mail.Models.Result<SentEmail>>>([]));

        await service.AddAsync(app);

        mailServerOrchestrationServiceMock.VerifyAll();
        queuedEmailOrchestrationServiceMock.VerifyAll();
        sentEmailOrchestrationServiceMock.VerifyAll();
    }
}
