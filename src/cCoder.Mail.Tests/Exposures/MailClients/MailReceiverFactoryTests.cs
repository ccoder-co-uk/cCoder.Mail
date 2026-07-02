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
        pop3ReceiverProviderMock = new Mock<IMailReceiverProvider>(MockBehavior.Strict);
        graphReceiverProviderMock = new Mock<IMailReceiverProvider>(MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();

        pop3ReceiverProviderMock.SetupGet(provider => provider.ProviderName)
            .Returns(MailProviderNames.Pop3);
        graphReceiverProviderMock.SetupGet(provider => provider.ProviderName)
            .Returns(MailProviderNames.MicrosoftGraph);

        mailReceiverFactory = new MailReceiverFactory(
        [
            pop3ReceiverProviderMock.Object,
            graphReceiverProviderMock.Object,
        ], mailConfiguration);
    }
}
