// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXTEST005

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Brokers.Configurations;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    private readonly Mock<IMailSenderClientBroker> mailSenderClientBrokerMock;
    private readonly Mock<IMailConfigurationBroker> mailConfigurationBrokerMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly MailSendingService mailSendingService;

    public MailSendingServiceTests()
    {
        mailSenderClientBrokerMock = new Mock<IMailSenderClientBroker>(behavior: MockBehavior.Strict);
        mailConfigurationBrokerMock = new Mock<IMailConfigurationBroker>(behavior: MockBehavior.Strict);
        loggerMock = new Mock<ILoggingBroker>(behavior: MockBehavior.Strict);
        mailSendingService = new MailSendingService(
            mailSenderClientBroker: mailSenderClientBrokerMock.Object,
            mailConfigurationBroker: mailConfigurationBrokerMock.Object,
            logger: loggerMock.Object);
    }
}

#pragma warning restore STXTEST005