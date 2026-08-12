// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public sealed partial class ReceivedEmailOrchestrationServiceTests
{
    private readonly Mock<IReceivedEmailProcessingService> receivedMock = new();
    private readonly Mock<IMailReceivingProcessingService> receivingMock = new();
    private readonly ReceivedEmailOrchestrationService service;

    public ReceivedEmailOrchestrationServiceTests() => service = new(receivedMock.Object, receivingMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsAsyncShouldReturnWhetherEmailExistsAsync(bool exists)
    {
        receivedMock.Setup(x => x.GetAllReceivedEmail(true)).Returns((exists ? new[] { new ReceivedEmail { Id = 1 } } : []).AsQueryable());
        (await service.ExistsAsync(1)).Should().Be(exists);
    }

    [Fact]
    public async Task AddReceivedEmailAsyncShouldDelegateAsync()
    {
        var email = new ReceivedEmail { Id = 1 }; receivedMock.Setup(x => x.AddReceivedEmailAsync(email)).ReturnsAsync(email);
        (await service.AddReceivedEmailAsync(email)).Should().BeSameAs(email);
    }

    [Fact]
    public async Task UpdateReceivedEmailAsyncShouldDelegateAsync()
    {
        var email = new ReceivedEmail { Id = 1 }; receivedMock.Setup(x => x.UpdateReceivedEmailAsync(email)).ReturnsAsync(email);
        (await service.UpdateReceivedEmailAsync(email)).Should().BeSameAs(email);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateAsync()
    {
        receivedMock.Setup(x => x.DeleteByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await service.DeleteByAppIdAsync(7); receivedMock.VerifyAll();
    }

    [Fact]
    public async Task ReceiveMailboxReceiveRequestAsyncShouldDelegateAsync()
    {
        var request = new MailboxReceiveRequest(); ReceivedEmail[] emails = [new() { Id = 1 }];
        receivingMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(request, CancellationToken.None)).ReturnsAsync(emails);
        (await service.ReceiveMailboxReceiveRequestAsync(request)).Should().BeSameAs(emails);
    }

    [Fact]
    public async Task ReceiveTopAsyncShouldDelegateAsync()
    {
        Guid receiverId = Guid.NewGuid(); ReceivedEmail[] emails = [new() { Id = 1 }];
        receivingMock.Setup(x => x.ReceiveTopAsync(receiverId, 3, CancellationToken.None)).ReturnsAsync(emails);
        (await service.ReceiveTopAsync(receiverId, 3)).Should().BeSameAs(emails);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005