// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Services.Foundations;
using cCoder.Mail.Providers.Services.Orchestrations;
using Moq;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class ImapMailReceiverServiceTests
{
    private readonly Mock<IImapMailReceiverBroker> receiverBrokerMock = new();
    private readonly Mock<IMailReceiverStorageBroker> storageBrokerMock = new();
    private readonly ImapMailReceiverOrchestrationService service;

    public ImapMailReceiverServiceTests() =>
        service = new ImapMailReceiverOrchestrationService(
            imapMailboxService: new ImapMailboxService(
                imapMailReceiverBroker: receiverBrokerMock.Object),
            mailReceiverProviderService: new MailReceiverProviderService(
                mailReceiverStorageBroker: storageBrokerMock.Object),
            mailMessageParsingService: new MailMessageParsingService(
                mailMessageParsingBroker: new MailMessageParsingBroker()));
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005