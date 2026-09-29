// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Foundations;

public sealed partial class MailProviderCatalogServiceTests
{
    [Fact]
    public void Senders_WhenClientSupportsSending_ReturnEveryProviderName()
    {
        // Given
        Mock<IMailClient> senderMock = CreateClient(
            names: ["SMTP", "Alias"],
            operations: [MailClientOperation.Send]);

        Mock<IMailClient> receiverMock = CreateClient(
            names: ["POP3"],
            operations: [MailClientOperation.Receive]);

        MailProviderCatalogService service = CreateService(
            mailClients:
            [
                senderMock.Object,
                receiverMock.Object
            ]);

        // When
        MailProviderSummary[] providers = service.GetSenders();

        // Then
        Assert.Equal(
            expected: ["SMTP", "Alias"],
            actual: providers
                .Select(selector: provider => provider.Name)
                .ToArray());

        providers.Should()
            .OnlyContain(
                predicate: provider =>
                    provider.ProviderName == "SMTP" &&
                    provider.Direction == "Sender");
    }

    [Fact]
    public void Receivers_WhenClientSupportsReceiving_ReturnEveryProviderName()
    {
        // Given
        Mock<IMailClient> receiverMock = CreateClient(
            names: ["IMAP", "Alias"],
            operations: [MailClientOperation.Receive]);

        Mock<IMailClient> senderMock = CreateClient(
            names: ["SMTP"],
            operations: [MailClientOperation.Send]);

        MailProviderCatalogService service = CreateService(
            mailClients:
            [
                receiverMock.Object,
                senderMock.Object
            ]);

        // When
        MailProviderSummary[] providers = service.GetReceivers();

        // Then
        Assert.Equal(
            expected: ["IMAP", "Alias"],
            actual: providers
                .Select(selector: provider => provider.Name)
                .ToArray());

        providers.Should()
            .OnlyContain(
                predicate: provider =>
                    provider.ProviderName == "IMAP" &&
                    provider.Direction == "Receiver");
    }

    [Fact]
    public void Catalogs_WhenClientSupportsBothOperations_IncludeClientInBoth()
    {
        // Given
        Mock<IMailClient> clientMock = CreateClient(
            names: ["Graph"],
            operations:
            [
                MailClientOperation.Send,
                MailClientOperation.Receive
            ]);

        MailProviderCatalogService service = CreateService(
            mailClients: [clientMock.Object]);

        // When
        MailProviderSummary[] senders = service.GetSenders();
        MailProviderSummary[] receivers = service.GetReceivers();

        // Then
        senders.Should()
            .ContainSingle();

        receivers.Should()
            .ContainSingle();
    }

    private static Mock<IMailClient> CreateClient(
        string[] names,
        MailClientOperation[] operations)
    {
        Mock<IMailClient> clientMock = new();

        clientMock.Setup(expression: client => client.GetProviderNames())
            .Returns(value: names);

        clientMock.Setup(expression: client => client.GetSupportedOperations())
            .Returns(value: operations);

        return clientMock;
    }

    private static MailProviderCatalogService CreateService(
        params IMailClient[] mailClients)
    {
        Mock<IMailProviderCatalogBroker> brokerMock = new();

        brokerMock.Setup(expression: broker => broker.SelectAllMailClients())
            .Returns(value: mailClients);

        return new MailProviderCatalogService(
            mailProviderCatalogBroker: brokerMock.Object);
    }
}