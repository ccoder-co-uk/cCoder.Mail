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

public partial class Pop3MailReceiverServiceTests
{
    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldParsePlainMessageAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        SetupRawMessages(
            [
                "Message-ID: message-1",
                "From: sender@example.test",
                "To: receiver@example.test",
                "Cc: copy@example.test",
                "Subject: Plain subject",
                "Date: Tue, 12 Aug 2026 12:00:00 +0000",
                "Content-Type: text/plain",
                "",
                "Plain body"
            ]);

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 10);

        result.Should().ContainSingle();
        result[0].Subject.Should().Be("Plain subject");
        result[0].Content.Should().Be("Plain body");
        result[0].IsBodyHtml.Should().BeFalse();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldDecodeEncodedContentAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        SetupRawMessages(
            [
                "Subject: =?UTF-8?B?RW5jb2RlZCBzdWJqZWN0?=",
                "Content-Type: text/html; charset=utf-8",
                "Content-Transfer-Encoding: quoted-printable",
                "",
                "Hello=20world"
            ]);

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 1);

        result[0].Subject.Should().Be("Encoded subject");
        result[0].Content.Should().Be("Hello world");
        result[0].IsBodyHtml.Should().BeTrue();
    }

    [Fact]
    public async Task ReceiveMailReceiverAsyncShouldPreferMultipartHtmlAsync()
    {
        Guid receiverId = Guid.NewGuid();
        SetupReceiver(receiverId);
        SetupRawMessages(
            [
                "Subject: Multipart",
                "Content-Type: multipart/alternative; boundary=boundary-1",
                "",
                "--boundary-1",
                "Content-Type: text/plain",
                "",
                "Plain part",
                "--boundary-1",
                "Content-Type: text/html",
                "Content-Transfer-Encoding: base64",
                "",
                "PGI+SHRtbCBwYXJ0PC9iPg==",
                "--boundary-1--"
            ]);

        ReceivedEmail[] result = await service.ReceiveMailReceiverAsync(receiverId, 1);

        result[0].Content.Should().Be("<b>Html part</b>");
        result[0].IsBodyHtml.Should().BeTrue();
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
            ProviderName = "POP3",
            Host = "pop.example.test",
            Port = 995,
            EnableSSL = true,
            User = "user",
            Password = "password"
        };

        storageBrokerMock.Setup(expression: broker => broker.SelectMailReceiverByIdAsync(
                receiverId,
                CancellationToken.None))
            .ReturnsAsync(value: receiver);
    }

    private void SetupRawMessages(string[] message) =>
        receiverBrokerMock.Setup(expression: broker => broker.ReceiveAsync(
                It.IsAny<MailboxReceiveRequest>(),
                CancellationToken.None))
            .ReturnsAsync(value: [message]);
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005