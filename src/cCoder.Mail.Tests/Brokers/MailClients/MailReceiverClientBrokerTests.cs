// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Exposures.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailReceiverClientBrokerTests
{
    private readonly Mock<IMailReceiverFactory> mailReceiverFactoryMock;
    private readonly Mock<IMailReceiverProvider> mailReceiverProviderMock;
    private readonly MailReceiverClientBroker mailReceiverClientBroker;

    public MailReceiverClientBrokerTests()
    {
        mailReceiverFactoryMock = new Mock<IMailReceiverFactory>(behavior: MockBehavior.Strict);
        mailReceiverProviderMock = new Mock<IMailReceiverProvider>(behavior: MockBehavior.Strict);
        mailReceiverClientBroker = new MailReceiverClientBroker(mailReceiverFactory: mailReceiverFactoryMock.Object);
    }
}