// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXTEST005

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    private readonly Mock<IMailSenderClientBroker> mailSenderClientBrokerMock;
    private readonly Mock<IMailConfigurationExposure> mailConfigurationExposureMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly MailSendingService mailSendingService;

    public MailSendingServiceTests()
    {
        mailSenderClientBrokerMock = new Mock<IMailSenderClientBroker>(behavior: MockBehavior.Strict);
        mailConfigurationExposureMock = new Mock<IMailConfigurationExposure>(behavior: MockBehavior.Strict);
        loggerMock = new Mock<ILoggingBroker>(behavior: MockBehavior.Strict);
        mailSendingService = new MailSendingService(
            mailSenderClientBroker: mailSenderClientBrokerMock.Object,
            mailConfigurationExposure: mailConfigurationExposureMock.Object,
            logger: loggerMock.Object);
    }
}

#pragma warning restore STXTEST005