// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using cCoder.Mail.Providers.Services.Orchestrations;
using Moq;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class MicrosoftGraphMailReceiverServiceTests
{
    private readonly Mock<IMailReceiverStorageBroker> storageBrokerMock = new();
    private readonly Mock<IMicrosoftGraphBroker> graphBrokerMock = new();
    private readonly MailProviderConfiguration configuration = new();
    private readonly MicrosoftGraphMailReceiverOrchestrationService service;

    public MicrosoftGraphMailReceiverServiceTests() =>
        service = new MicrosoftGraphMailReceiverOrchestrationService(
            configuration: configuration,
            mailReceiverProviderService: new MailReceiverProviderService(
                mailReceiverStorageBroker: storageBrokerMock.Object),
            microsoftGraphMailboxService: new MicrosoftGraphMailboxService(
                microsoftGraphBroker: graphBrokerMock.Object),
            microsoftGraphMessageService: new MicrosoftGraphMessageService(
                mailMessageParsingBroker: new MailMessageParsingBroker()));
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005