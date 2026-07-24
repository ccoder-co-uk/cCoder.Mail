// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Exposures;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class Pop3MailReceiverServiceTests
{
    private readonly Mock<IPop3MailReceiverBroker> pop3MailReceiverBrokerMock;
    private readonly MailConfiguration mailConfiguration;
    private readonly Mock<IMailConfigurationExposure> mailConfigurationExposureMock;
    private readonly Pop3MailReceiverService pop3MailReceiverService;

    public Pop3MailReceiverServiceTests()
    {
        pop3MailReceiverBrokerMock = new Mock<IPop3MailReceiverBroker>(behavior: MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();
        mailConfigurationExposureMock = new Mock<IMailConfigurationExposure>(behavior: MockBehavior.Strict);

        mailConfigurationExposureMock.Setup(expression: exposure => exposure.GetMailConfiguration())
            .Returns(value: mailConfiguration);

        pop3MailReceiverService = new Pop3MailReceiverService(
pop3MailReceiverBroker: pop3MailReceiverBrokerMock.Object,
mailConfigurationExposure: mailConfigurationExposureMock.Object);
    }
}