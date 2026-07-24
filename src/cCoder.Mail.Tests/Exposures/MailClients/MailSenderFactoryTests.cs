// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies.MailClients;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class MailSenderFactoryTests
{
    private readonly Mock<IMailSenderProvider> smtpSenderProviderMock;
    private readonly Mock<IMailSenderProvider> graphSenderProviderMock;
    private readonly MailConfiguration mailConfiguration;
    private readonly MailSenderFactory mailSenderFactory;

    public MailSenderFactoryTests()
    {
        smtpSenderProviderMock = new Mock<IMailSenderProvider>(behavior: MockBehavior.Strict);
        graphSenderProviderMock = new Mock<IMailSenderProvider>(behavior: MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();

        smtpSenderProviderMock.SetupGet(expression: provider => provider.ProviderName)
            .Returns(value: MailProviderNames.Smtp);

        graphSenderProviderMock.SetupGet(expression: provider => provider.ProviderName)
            .Returns(value: MailProviderNames.MicrosoftGraph);

        mailSenderFactory = new MailSenderFactory(
senders: [
            smtpSenderProviderMock.Object,
            graphSenderProviderMock.Object,
        ], mailConfiguration: mailConfiguration);
    }
}