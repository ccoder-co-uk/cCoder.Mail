// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using Moq;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class MicrosoftGraphMailSenderServiceTests
{
    private readonly Mock<IMicrosoftGraphBroker> graphBrokerMock = new();
    private readonly MailProviderConfiguration configuration = new();
    private readonly MicrosoftGraphMailSenderService service;

    public MicrosoftGraphMailSenderServiceTests() =>
        service = new MicrosoftGraphMailSenderService(configuration, graphBrokerMock.Object);
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005