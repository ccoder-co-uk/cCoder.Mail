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

public partial class MailReceivingServiceTests
{
    private readonly Mock<IMailReceiverClientBroker> mailReceiverClientBrokerMock;
    private readonly Mock<IMailConfigurationBroker> mailConfigurationBrokerMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly MailReceivingService mailReceivingService;

    public MailReceivingServiceTests()
    {
        mailReceiverClientBrokerMock = new Mock<IMailReceiverClientBroker>(behavior: MockBehavior.Strict);
        mailConfigurationBrokerMock = new Mock<IMailConfigurationBroker>(behavior: MockBehavior.Strict);
        loggerMock = new Mock<ILoggingBroker>(behavior: MockBehavior.Strict);
        mailReceivingService = new MailReceivingService(
            mailReceiverClientBroker: mailReceiverClientBrokerMock.Object,
            mailConfigurationBroker: mailConfigurationBrokerMock.Object,
            logger: loggerMock.Object);
    }
}

#pragma warning restore STXTEST005