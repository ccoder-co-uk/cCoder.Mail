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
        smtpSenderProviderMock = new Mock<IMailSenderProvider>(MockBehavior.Strict);
        graphSenderProviderMock = new Mock<IMailSenderProvider>(MockBehavior.Strict);
        mailConfiguration = new MailConfiguration();

        smtpSenderProviderMock.SetupGet(provider => provider.ProviderName)
            .Returns(MailProviderNames.Smtp);
        graphSenderProviderMock.SetupGet(provider => provider.ProviderName)
            .Returns(MailProviderNames.MicrosoftGraph);

        mailSenderFactory = new MailSenderFactory(
        [
            smtpSenderProviderMock.Object,
            graphSenderProviderMock.Object,
        ], mailConfiguration);
    }
}
