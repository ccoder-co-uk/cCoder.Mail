using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class SmtpMailSenderProviderTests
{
    private readonly Mock<ISmtpMailSenderService> smtpMailSenderServiceMock;
    private readonly SmtpMailSenderProvider smtpMailSenderProvider;

    public SmtpMailSenderProviderTests()
    {
        smtpMailSenderServiceMock = new Mock<ISmtpMailSenderService>(MockBehavior.Strict);
        smtpMailSenderProvider = new SmtpMailSenderProvider(smtpMailSenderServiceMock.Object);
    }
}
