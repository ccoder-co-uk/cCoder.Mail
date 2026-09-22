// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Services.Foundations;
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
        Mock<IMailProviderCatalogBroker> brokerMock = CreateBroker(senderMock.Object, receiverMock.Object);
        var catalog = new MailProviderCatalog(
            new MailProviderCatalogService(brokerMock.Object));

        Models.MailProviderSummary[] providers = catalog.GetSenders();

        providers.Select(x => x.Name).Should().Equal("SMTP", "Alias");
        providers.Should().OnlyContain(x => x.ProviderName == "SMTP" && x.Direction == "Sender");
    }

    [Fact]
    public void GetReceiversShouldReturnEveryNameForReceivingClients()
    {
        Mock<IMailClient> receiverMock = CreateClient(["IMAP", "Alias"], [MailClientOperation.Receive]);
        Mock<IMailClient> senderMock = CreateClient(["SMTP"], [MailClientOperation.Send]);
        Mock<IMailProviderCatalogBroker> brokerMock = CreateBroker(receiverMock.Object, senderMock.Object);
        var catalog = new MailProviderCatalog(
            new MailProviderCatalogService(brokerMock.Object));

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
        Mock<IMailProviderCatalogBroker> brokerMock = CreateBroker(clientMock.Object);
        var catalog = new MailProviderCatalog(
            new MailProviderCatalogService(brokerMock.Object));

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

    private static Mock<IMailProviderCatalogBroker> CreateBroker(
        params IMailClient[] mailClients)
    {
        Mock<IMailProviderCatalogBroker> brokerMock = new();
        brokerMock.Setup(broker => broker.SelectAllMailClients())
            .Returns(mailClients);

        return brokerMock;
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005