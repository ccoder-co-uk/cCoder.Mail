// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class QueuedEmailOrchestrationServiceTests
{
    private readonly Mock<IQueuedEmailProcessingService> queuedEmailProcessingServiceMock;
    private readonly Mock<IQueuedEmailEventProcessingService> queuedEmailEventProcessingServiceMock;
    private readonly QueuedEmailOrchestrationService orchestrationService;

    public QueuedEmailOrchestrationServiceTests()
    {
        queuedEmailProcessingServiceMock = new Mock<IQueuedEmailProcessingService>(behavior: MockBehavior.Strict);
        queuedEmailEventProcessingServiceMock = new Mock<IQueuedEmailEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new QueuedEmailOrchestrationService(
processingService: queuedEmailProcessingServiceMock.Object,
eventService: queuedEmailEventProcessingServiceMock.Object
        );
    }

    private static QueuedEmail CreateRandomQueuedEmail() =>
        Builder<QueuedEmail>.CreateNew()
        .Build();
}