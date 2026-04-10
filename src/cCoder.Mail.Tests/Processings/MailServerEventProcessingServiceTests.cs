using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerEventProcessingServiceTests
{
    private readonly Mock<IMailServerEventService> mailServerEventServiceMock;
    private readonly MailServerEventProcessingService service;

    public MailServerEventProcessingServiceTests()
    {
        mailServerEventServiceMock = new Mock<IMailServerEventService>(MockBehavior.Strict);
        service = new MailServerEventProcessingService(mailServerEventServiceMock.Object);
    }

    private static MailServer CreateRandomMailServer() =>
        Builder<MailServer>.CreateNew().Build();
}











