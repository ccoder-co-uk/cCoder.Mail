// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class MailReceiverFactoryTests
{
    private readonly Mock<IMailReceiverProvider> pop3ReceiverProviderMock;
    private readonly Mock<IMailReceiverProvider> graphReceiverProviderMock;
    private readonly MailConfiguration mailConfiguration;
    private readonly MailReceiverFactory mailReceiverFactory;

    public MailReceiverFactoryTests()
    {
        pop3ReceiverProviderMock = new Mock<IMailReceiverProvider>(behavior: MockBehavior.Strict);
        graphReceiverProviderMock = new Mock<IMailReceiverProvider>(behavior: MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();

        pop3ReceiverProviderMock.SetupGet(expression: provider => provider.ProviderName)
            .Returns(value: MailProviderNames.Pop3);

        graphReceiverProviderMock.SetupGet(expression: provider => provider.ProviderName)
            .Returns(value: MailProviderNames.MicrosoftGraph);

        mailReceiverFactory = new MailReceiverFactory(
receivers: [
            pop3ReceiverProviderMock.Object,
            graphReceiverProviderMock.Object,
        ], mailConfiguration: mailConfiguration);
    }
}