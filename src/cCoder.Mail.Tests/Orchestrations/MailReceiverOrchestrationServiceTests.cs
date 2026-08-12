// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Services.Processings;
using Moq;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailReceiverOrchestrationServiceTests
{
    private readonly Mock<IMailReceiverProcessingService> mailReceiverProcessingServiceMock;
    private readonly Mock<IReceivedEmailProcessingService> receivedEmailProcessingServiceMock;
    private readonly Mock<IMailReceivingProcessingService> mailReceivingProcessingServiceMock;
    private readonly MailReceiverOrchestrationService mailReceiverOrchestrationService;

    public MailReceiverOrchestrationServiceTests()
    {
        mailReceiverProcessingServiceMock = new Mock<IMailReceiverProcessingService>();
        receivedEmailProcessingServiceMock = new Mock<IReceivedEmailProcessingService>();
        mailReceivingProcessingServiceMock = new Mock<IMailReceivingProcessingService>();

        mailReceiverOrchestrationService = new MailReceiverOrchestrationService(
            mailReceiverProcessingService: mailReceiverProcessingServiceMock.Object,
            receivedEmailProcessingService: receivedEmailProcessingServiceMock.Object,
            mailReceivingProcessingService: mailReceivingProcessingServiceMock.Object);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005