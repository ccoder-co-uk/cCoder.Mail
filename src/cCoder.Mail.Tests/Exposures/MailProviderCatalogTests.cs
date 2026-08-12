// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Exposures.MailClients;

public sealed partial class MailProviderCatalogTests
{
    [Fact]
    public void GetSendersShouldReturnEveryNameForSendingClients()
    {
        Mock<IMailClient> senderMock = CreateClient(["SMTP", "Alias"], [MailClientOperation.Send]);
        Mock<IMailClient> receiverMock = CreateClient(["POP3"], [MailClientOperation.Receive]);
        var catalog = new MailProviderCatalog([senderMock.Object, receiverMock.Object]);

        Models.MailProviderSummary[] providers = catalog.GetSenders();

        providers.Select(x => x.Name).Should().Equal("SMTP", "Alias");
        providers.Should().OnlyContain(x => x.ProviderName == "SMTP" && x.Direction == "Sender");
    }

    [Fact]
    public void GetReceiversShouldReturnEveryNameForReceivingClients()
    {
        Mock<IMailClient> receiverMock = CreateClient(["IMAP", "Alias"], [MailClientOperation.Receive]);
        Mock<IMailClient> senderMock = CreateClient(["SMTP"], [MailClientOperation.Send]);
        var catalog = new MailProviderCatalog([receiverMock.Object, senderMock.Object]);

        Models.MailProviderSummary[] providers = catalog.GetReceivers();

        providers.Select(x => x.Name).Should().Equal("IMAP", "Alias");
        providers.Should().OnlyContain(x => x.ProviderName == "IMAP" && x.Direction == "Receiver");
    }

    [Fact]
    public void ClientSupportingBothOperationsShouldAppearInBothCatalogs()
    {
        Mock<IMailClient> clientMock = CreateClient(
            ["Graph"],
            [MailClientOperation.Send, MailClientOperation.Receive]);
        var catalog = new MailProviderCatalog([clientMock.Object]);

        catalog.GetSenders().Should().ContainSingle();
        catalog.GetReceivers().Should().ContainSingle();
    }

    private static Mock<IMailClient> CreateClient(string[] names, MailClientOperation[] operations)
    {
        Mock<IMailClient> clientMock = new();
        clientMock.Setup(x => x.GetProviderNames()).Returns(names);
        clientMock.Setup(x => x.GetSupportedOperations()).Returns(operations);
        return clientMock;
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005