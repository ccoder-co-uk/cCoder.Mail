// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies.MailClients;
using cCoder.Mail.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class Pop3MailReceiverProviderTests
{
    private readonly Mock<IPop3MailReceiverService> pop3MailReceiverServiceMock;
    private readonly Pop3MailReceiverProvider pop3MailReceiverProvider;

    public Pop3MailReceiverProviderTests()
    {
        pop3MailReceiverServiceMock = new Mock<IPop3MailReceiverService>(behavior: MockBehavior.Strict);
        pop3MailReceiverProvider = new Pop3MailReceiverProvider(pop3MailReceiverService: pop3MailReceiverServiceMock.Object);
    }
}