// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Processings;
using Moq;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailSenderOrchestrationServiceTests
{
    private readonly Mock<IMailSendingProcessingService> mailSendingProcessingServiceMock;
    private readonly MailSenderOrchestrationService mailSenderOrchestrationService;

    public MailSenderOrchestrationServiceTests()
    {
        mailSendingProcessingServiceMock = new Mock<IMailSendingProcessingService>();

        mailSenderOrchestrationService = new MailSenderOrchestrationService(
            queuedEmailProcessingService: new Mock<IQueuedEmailProcessingService>().Object,
            mailSendingProcessingService: mailSendingProcessingServiceMock.Object,
            mailSenderProcessingService: new Mock<IMailSenderProcessingService>().Object);
    }
}