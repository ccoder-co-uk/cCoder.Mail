using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailServerOrchestrationServiceTests
{
    private readonly Mock<IMailServerProcessingService> mailServerProcessingServiceMock;
    private readonly Mock<IMailServerEventProcessingService> mailServerEventProcessingServiceMock;
    private readonly MailServerOrchestrationService orchestrationService;

    public MailServerOrchestrationServiceTests()
    {
        mailServerProcessingServiceMock = new Mock<IMailServerProcessingService>(MockBehavior.Strict);
        mailServerEventProcessingServiceMock = new Mock<IMailServerEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new MailServerOrchestrationService(
            mailServerProcessingServiceMock.Object,
            mailServerEventProcessingServiceMock.Object
        );
    }

    private static MailServer CreateRandomMailServer() => Builder<MailServer>.CreateNew().Build();
}









