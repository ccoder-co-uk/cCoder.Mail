// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Services.Foundations;
using Moq;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class ImapMailReceiverServiceTests
{
    private readonly Mock<IImapMailReceiverBroker> receiverBrokerMock = new();
    private readonly Mock<IMailReceiverStorageBroker> storageBrokerMock = new();
    private readonly ImapMailReceiverService service;

    public ImapMailReceiverServiceTests() =>
        service = new ImapMailReceiverService(
            imapMailReceiverBroker: receiverBrokerMock.Object,
            mailReceiverStorageBroker: storageBrokerMock.Object);
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005