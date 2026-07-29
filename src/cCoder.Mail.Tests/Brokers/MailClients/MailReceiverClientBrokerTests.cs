// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailReceiverClientBrokerTests
{
    private readonly Mock<IMailClientFactory> mailClientFactoryMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<IMailClient> mailClientMock =
        new(behavior: MockBehavior.Strict);
}