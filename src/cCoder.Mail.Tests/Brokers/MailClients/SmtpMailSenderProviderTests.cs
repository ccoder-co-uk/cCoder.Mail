using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

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
