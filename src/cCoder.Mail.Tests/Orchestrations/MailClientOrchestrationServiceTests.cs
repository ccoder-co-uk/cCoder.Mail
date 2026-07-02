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
        mailSendingServiceMock = new Mock<IMailSendingService>(MockBehavior.Strict);
        mailReceivingServiceMock = new Mock<IMailReceivingService>(MockBehavior.Strict);
        mailClientOrchestrationService = new MailClientOrchestrationService(
            mailSendingServiceMock.Object,
            mailReceivingServiceMock.Object);
    }
}
