// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using Moq;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class MicrosoftGraphMailReceiverServiceTests
{
    private readonly Mock<IMailReceiverStorageBroker> storageBrokerMock = new();
    private readonly Mock<IMicrosoftGraphBroker> graphBrokerMock = new();
    private readonly MailProviderConfiguration configuration = new();
    private readonly MicrosoftGraphMailReceiverService service;

    public MicrosoftGraphMailReceiverServiceTests() =>
        service = new MicrosoftGraphMailReceiverService(
            configuration,
            storageBrokerMock.Object,
            graphBrokerMock.Object);
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005