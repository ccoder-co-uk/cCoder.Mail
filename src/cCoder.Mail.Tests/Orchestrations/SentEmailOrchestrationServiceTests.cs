using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class SentEmailOrchestrationServiceTests
{
    private readonly Mock<ISentEmailProcessingService> sentEmailProcessingServiceMock;
    private readonly Mock<ISentEmailEventProcessingService> sentEmailEventProcessingServiceMock;
    private readonly SentEmailOrchestrationService orchestrationService;

    public SentEmailOrchestrationServiceTests()
    {
        sentEmailProcessingServiceMock = new Mock<ISentEmailProcessingService>(MockBehavior.Strict);
        sentEmailEventProcessingServiceMock = new Mock<ISentEmailEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new SentEmailOrchestrationService(
            sentEmailProcessingServiceMock.Object,
            sentEmailEventProcessingServiceMock.Object
        );
    }

    private static SentEmail CreateRandomSentEmail() => Builder<SentEmail>.CreateNew().Build();
}









