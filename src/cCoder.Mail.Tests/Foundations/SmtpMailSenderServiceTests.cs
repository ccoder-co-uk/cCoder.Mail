// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SmtpMailSenderServiceTests
{
    private readonly Mock<ISmtpMailSenderBroker> smtpMailSenderBrokerMock;
    private readonly SmtpMailSenderService smtpMailSenderService;

    public SmtpMailSenderServiceTests()
    {
        smtpMailSenderBrokerMock = new Mock<ISmtpMailSenderBroker>(behavior: MockBehavior.Strict);
        smtpMailSenderService = new SmtpMailSenderService(smtpMailSenderBroker: smtpMailSenderBrokerMock.Object);
    }
}