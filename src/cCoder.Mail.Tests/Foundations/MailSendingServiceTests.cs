// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    private readonly Mock<IMailSenderClientBroker> mailSenderClientBrokerMock;
    private readonly MailSendingService mailSendingService;

    public MailSendingServiceTests()
    {
        mailSenderClientBrokerMock = new Mock<IMailSenderClientBroker>(behavior: MockBehavior.Strict);
        mailSendingService = new MailSendingService(
            mailSenderClientBroker: mailSenderClientBrokerMock.Object,
            mailConfigurationExposure: Mock.Of<IMailConfigurationExposure>(),
            logger: Mock.Of<ILoggingBroker>());
    }
}