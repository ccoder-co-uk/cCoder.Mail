// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class MicrosoftGraphMailClientTests
{
    [Fact]
    public async Task OperationsShouldDelegateToGraphServicesAsync()
    {
        Mock<IMicrosoftGraphMailSenderService> senderMock = new(); Mock<IMicrosoftGraphMailReceiverService> receiverMock = new();
        var email = new QueuedEmail(); Guid id = Guid.NewGuid(); ReceivedEmail[] received = [new()];
        senderMock.Setup(x => x.SendQueuedEmailAsync(email, CancellationToken.None)).Returns(Task.CompletedTask);
        receiverMock.Setup(x => x.ReceiveMailReceiverAsync(id, 2, CancellationToken.None)).ReturnsAsync(received);
        var client = new MicrosoftGraphMailClient(senderMock.Object, receiverMock.Object);
        client.GetProviderNames().Should().HaveCount(4); client.GetSupportedOperations().Should().HaveCount(2);
        await client.SendAsync(email); (await client.ReceiveAsync(id, 2)).Should().BeSameAs(received);
    }
}

public sealed partial class ImapMailClientTests
{
    [Fact]
    public async Task OperationsShouldExposeReceiveOnlyAndDelegateAsync()
    {
        Mock<IImapMailReceiverService> receiverMock = new(); Guid id = Guid.NewGuid(); ReceivedEmail[] emails = [new()];
        receiverMock.Setup(x => x.ReceiveMailReceiverAsync(id, 2, CancellationToken.None)).ReturnsAsync(emails);
        var client = new ImapMailClient(receiverMock.Object);
        client.GetProviderNames().Should().Equal(MailProviderNames.Imap); client.GetSupportedOperations().Should().Equal(MailClientOperation.Receive);
        (await client.ReceiveAsync(id, 2)).Should().BeSameAs(emails);
        Func<Task> action = () => client.SendAsync(new QueuedEmail()); await action.Should().ThrowAsync<UnsupportedMailClientOperationException>();
    }
}

public sealed partial class PopMailClientTests
{
    [Fact]
    public async Task OperationsShouldExposeReceiveOnlyAndDelegateAsync()
    {
        Mock<IPop3MailReceiverService> receiverMock = new(); Guid id = Guid.NewGuid(); ReceivedEmail[] emails = [new()];
        receiverMock.Setup(x => x.ReceiveMailReceiverAsync(id, 2, CancellationToken.None)).ReturnsAsync(emails);
        var client = new PopMailClient(receiverMock.Object);
        client.GetProviderNames().Should().Equal(MailProviderNames.Pop3); client.GetSupportedOperations().Should().Equal(MailClientOperation.Receive);
        (await client.ReceiveAsync(id, 2)).Should().BeSameAs(emails);
        Func<Task> action = () => client.SendAsync(new QueuedEmail()); await action.Should().ThrowAsync<UnsupportedMailClientOperationException>();
    }
}

public sealed partial class SmtpMailClientTests
{
    [Fact]
    public async Task OperationsShouldExposeSendOnlyAndDelegateAsync()
    {
        Mock<ISmtpMailSenderService> senderMock = new(); var email = new QueuedEmail();
        senderMock.Setup(x => x.SendQueuedEmailAsync(email, CancellationToken.None)).Returns(Task.CompletedTask);
        var client = new SmtpMailClient(senderMock.Object);
        client.GetProviderNames().Should().Equal(MailProviderNames.Smtp); client.GetSupportedOperations().Should().Equal(MailClientOperation.Send);
        await client.SendAsync(email);
        Func<Task> action = () => client.ReceiveAsync(Guid.NewGuid(), 2); await action.Should().ThrowAsync<UnsupportedMailClientOperationException>();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005