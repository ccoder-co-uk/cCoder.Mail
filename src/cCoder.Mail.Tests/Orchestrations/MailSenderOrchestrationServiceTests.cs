// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Services.Processings;
using Moq;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailSenderOrchestrationServiceTests
{
    private readonly Mock<IQueuedEmailProcessingService> queuedEmailProcessingServiceMock;
    private readonly Mock<IMailSendingProcessingService> mailSendingProcessingServiceMock;
    private readonly Mock<IMailSenderProcessingService> mailSenderProcessingServiceMock;
    private readonly MailSenderOrchestrationService mailSenderOrchestrationService;

    public MailSenderOrchestrationServiceTests()
    {
        queuedEmailProcessingServiceMock = new Mock<IQueuedEmailProcessingService>();
        mailSendingProcessingServiceMock = new Mock<IMailSendingProcessingService>();
        mailSenderProcessingServiceMock = new Mock<IMailSenderProcessingService>();

        mailSenderOrchestrationService = new MailSenderOrchestrationService(
            queuedEmailProcessingService: queuedEmailProcessingServiceMock.Object,
            mailSendingProcessingService: mailSendingProcessingServiceMock.Object,
            mailSenderProcessingService: mailSenderProcessingServiceMock.Object);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005