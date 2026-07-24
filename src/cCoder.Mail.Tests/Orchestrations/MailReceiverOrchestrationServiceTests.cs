// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Processings;
using Moq;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailReceiverOrchestrationServiceTests
{
    private readonly Mock<IMailReceivingProcessingService> mailReceivingProcessingServiceMock;
    private readonly MailReceiverOrchestrationService mailReceiverOrchestrationService;

    public MailReceiverOrchestrationServiceTests()
    {
        mailReceivingProcessingServiceMock = new Mock<IMailReceivingProcessingService>();

        mailReceiverOrchestrationService = new MailReceiverOrchestrationService(
            mailReceiverProcessingService: new Mock<IMailReceiverProcessingService>().Object,
            receivedEmailProcessingService: new Mock<IReceivedEmailProcessingService>().Object,
            mailReceivingProcessingService: mailReceivingProcessingServiceMock.Object);
    }
}