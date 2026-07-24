// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Exposures.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailSenderClientBrokerTests
{
    private readonly Mock<IMailSenderFactory> mailSenderFactoryMock;
    private readonly Mock<IMailSenderProvider> mailSenderProviderMock;
    private readonly MailSenderClientBroker mailSenderClientBroker;

    public MailSenderClientBrokerTests()
    {
        mailSenderFactoryMock = new Mock<IMailSenderFactory>(behavior: MockBehavior.Strict);
        mailSenderProviderMock = new Mock<IMailSenderProvider>(behavior: MockBehavior.Strict);
        mailSenderClientBroker = new MailSenderClientBroker(mailSenderFactory: mailSenderFactoryMock.Object);
    }
}