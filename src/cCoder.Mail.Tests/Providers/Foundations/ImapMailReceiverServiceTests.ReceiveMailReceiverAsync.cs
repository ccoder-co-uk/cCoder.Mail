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

public partial class ImapMailReceiverServiceTests
{
    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldParseImapMessageAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);

        string rawMessage = string.Join(
            separator: "\r\n",
            value:
            [
                "IMAP envelope prefix",
                "From: sender@example.test",
                "To: receiver@example.test",
                "Cc: copy@example.test",
                "Subject: =?UTF-8?Q?Encoded_subject?=",
                "Date: Tue, 12 Aug 2026 12:00:00 +0000",
                "Content-Type: text/html",
                "",
                "<b>Body</b>)"
            ]);

        receiverBrokerMock.Setup(expression: broker => broker.ReceiveAsync(
                It.IsAny<MailboxReceiveRequest>(),
                CancellationToken.None))
            .ReturnsAsync(value: [rawMessage]);

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 10);

        result.Should().ContainSingle();
        result[0].Subject.Should().Be("Encoded subject");
        result[0].Content.Should().Be("<b>Body</b>");
        result[0].IsBodyHtml.Should().BeTrue();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldDecodeBase64SubjectAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        receiverBrokerMock.Setup(expression: broker => broker.ReceiveAsync(
                It.IsAny<MailboxReceiveRequest>(),
                CancellationToken.None))
            .ReturnsAsync(value:
            [
                "From: sender@example.test\nSubject: =?UTF-8?B?QmFzZTY0IHN1YmplY3Q=?=\n\nBody"
            ]);

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 1);

        result[0].Subject.Should().Be("Base64 subject");
        result[0].IsBodyHtml.Should().BeFalse();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldRejectMissingReceiverAsync()
    {
        Guid receiverId = Guid.NewGuid();
        storageBrokerMock.Setup(expression: broker => broker.SelectMailReceiverByIdAsync(
                receiverId,
                CancellationToken.None))
            .ReturnsAsync(value: null);

        Func<Task> action = async () => await service.ReceiveMailReceiverAsync(receiverId, 1);

        await action.Should().ThrowAsync<Exception>();
    }

    private void SetupReceiver(Guid receiverId)
    {
        var receiver = new MailReceiver
        {
            Id = receiverId,
            AppId = 7,
            ProviderName = "IMAP",
            Host = "imap.example.test",
            Port = 993,
            EnableSSL = true,
            User = "user",
            Password = "password"
        };

        storageBrokerMock.Setup(expression: broker => broker.SelectMailReceiverByIdAsync(
                receiverId,
                CancellationToken.None))
            .ReturnsAsync(value: receiver);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005