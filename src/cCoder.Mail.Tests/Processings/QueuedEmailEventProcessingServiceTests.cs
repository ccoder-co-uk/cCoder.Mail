using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailEventProcessingServiceTests
{
    private readonly Mock<IQueuedEmailEventService> queuedEmailEventServiceMock;
    private readonly QueuedEmailEventProcessingService service;

    public QueuedEmailEventProcessingServiceTests()
    {
        queuedEmailEventServiceMock = new Mock<IQueuedEmailEventService>(MockBehavior.Strict);
        service = new QueuedEmailEventProcessingService(queuedEmailEventServiceMock.Object);
    }

    private static QueuedEmail CreateRandomQueuedEmail() =>
        Builder<QueuedEmail>.CreateNew().Build();
}











