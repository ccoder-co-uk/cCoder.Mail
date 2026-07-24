// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class Pop3MailReceiverServiceTests
{
    private readonly Mock<IPop3MailReceiverBroker> pop3MailReceiverBrokerMock;
    private readonly MailConfiguration mailConfiguration;
    private readonly Pop3MailReceiverService pop3MailReceiverService;

    public Pop3MailReceiverServiceTests()
    {
        pop3MailReceiverBrokerMock = new Mock<IPop3MailReceiverBroker>(behavior: MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();

        pop3MailReceiverService = new Pop3MailReceiverService(
pop3MailReceiverBroker: pop3MailReceiverBrokerMock.Object,
mailConfiguration: mailConfiguration);
    }
}