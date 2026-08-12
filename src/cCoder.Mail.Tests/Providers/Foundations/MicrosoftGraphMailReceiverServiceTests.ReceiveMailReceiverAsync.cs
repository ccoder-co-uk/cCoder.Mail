// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class MicrosoftGraphMailReceiverServiceTests
{
    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldParseGraphMessagesAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        const string content = """
            {"value":[{"internetMessageId":"message-1","subject":"Subject","receivedDateTime":"2026-08-12T12:00:00Z","from":{"emailAddress":{"address":"from@example.test"}},"toRecipients":[{"emailAddress":{"address":"to@example.test"}}],"ccRecipients":[{"emailAddress":{"address":"cc@example.test"}}],"body":{"contentType":"html","content":"<b>Body</b>"}}]}
            """;

        graphBrokerMock.Setup(expression: broker => broker.ReceiveEmailAsync(
                It.IsAny<MailboxReceiveRequest>(),
                configuration,
                CancellationToken.None))
            .ReturnsAsync(value: new HttpClientBrokerResponse(true, content));

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 10);

        result.Should().ContainSingle();
        result[0].From.Should().Be("from@example.test");
        result[0].To.Should().Be("to@example.test");
        result[0].CC.Should().Be("cc@example.test");
        result[0].Content.Should().Be("<b>Body</b>");
        result[0].IsBodyHtml.Should().BeTrue();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldReturnEmptyWhenGraphHasNoMessagesAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        graphBrokerMock.Setup(expression: broker => broker.ReceiveEmailAsync(
                It.IsAny<MailboxReceiveRequest>(),
                configuration,
                CancellationToken.None))
            .ReturnsAsync(value: new HttpClientBrokerResponse(true, "{}"));

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 10);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldRejectGraphFailureAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        graphBrokerMock.Setup(expression: broker => broker.ReceiveEmailAsync(
                It.IsAny<MailboxReceiveRequest>(),
                configuration,
                CancellationToken.None))
            .ReturnsAsync(value: new HttpClientBrokerResponse(false, "failure"));

        Func<Task> action = async () => await service.ReceiveMailReceiverAsync(receiverId, 10);

        await action.Should().ThrowAsync<Exception>();
    }

    private void SetupReceiver(Guid receiverId)
    {
        var receiver = new MailReceiver
        {
            Id = receiverId,
            AppId = 7,
            ProviderName = "MicrosoftGraph",
            User = "user@example.test"
        };

        storageBrokerMock.Setup(expression: broker => broker.SelectMailReceiverByIdAsync(
                receiverId,
                CancellationToken.None))
            .ReturnsAsync(value: receiver);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005