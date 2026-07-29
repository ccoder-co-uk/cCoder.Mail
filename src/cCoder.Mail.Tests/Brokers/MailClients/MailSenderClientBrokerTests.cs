// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailSenderClientBrokerTests
{
    private readonly Mock<IMailClientFactory> mailClientFactoryMock;
    private readonly Mock<IMailClient> mailClientMock;
    private readonly MailSenderClientBroker mailSenderClientBroker;

    public MailSenderClientBrokerTests()
    {
        mailClientFactoryMock =
            new Mock<IMailClientFactory>(
                behavior: MockBehavior.Strict);
        mailClientMock =
            new Mock<IMailClient>(
                behavior: MockBehavior.Strict);
        mailSenderClientBroker =
            new MailSenderClientBroker(
                mailClientFactory:
                    mailClientFactoryMock.Object);
    }
}