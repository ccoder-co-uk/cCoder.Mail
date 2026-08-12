// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT004, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailReceiverProcessingServiceTests
{
    private readonly Mock<IMailReceiverService> serviceMock = new();
    private readonly MailReceiverProcessingService service;

    public MailReceiverProcessingServiceTests() => service = new(serviceMock.Object);

    [Fact] public void GetMailReceiverShouldDelegate() { Guid id = Guid.NewGuid(); var item = new MailReceiver { Id = id }; serviceMock.Setup(x => x.GetMailReceiver(id)).Returns(item); service.GetMailReceiver(id).Should().BeSameAs(item); }
    [Fact] public void GetAllMailReceiverShouldDelegate() { IQueryable<MailReceiver> items = Array.Empty<MailReceiver>().AsQueryable(); serviceMock.Setup(x => x.GetAllMailReceiver(true)).Returns(items); service.GetAllMailReceiver(true).Should().BeSameAs(items); }
    [Fact] public void GetEnabledShouldDelegate() { MailReceiver[] items = [new() { Id = Guid.NewGuid() }]; serviceMock.Setup(x => x.GetEnabled()).Returns(items); service.GetEnabled().Should().BeSameAs(items); }
    [Fact] public async Task AddMailReceiverAsyncShouldDelegateAsync() { var item = new MailReceiver { Id = Guid.NewGuid() }; serviceMock.Setup(x => x.AddMailReceiverAsync(item)).ReturnsAsync(item); (await service.AddMailReceiverAsync(item)).Should().BeSameAs(item); }
    [Fact] public async Task UpdateMailReceiverAsyncShouldDelegateAsync() { var item = new MailReceiver { Id = Guid.NewGuid() }; serviceMock.Setup(x => x.UpdateMailReceiverAsync(item)).ReturnsAsync(item); (await service.UpdateMailReceiverAsync(item)).Should().BeSameAs(item); }
    [Fact] public async Task DeleteAsyncShouldDelegateAsync() { Guid id = Guid.NewGuid(); serviceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(1); (await service.DeleteAsync(id)).Should().Be(1); }
    [Fact] public async Task DeleteByAppIdAsyncShouldDelegateAsync() { serviceMock.Setup(x => x.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask); await service.DeleteByAppIdAsync(7); serviceMock.VerifyAll(); }
    [Fact] public async Task DeleteAllMailReceiverAsyncShouldDelegateAsync() { MailReceiver[] items = [new() { Id = Guid.NewGuid() }]; serviceMock.Setup(x => x.DeleteAllMailReceiverAsync(items)).Returns(ValueTask.CompletedTask); await service.DeleteAllMailReceiverAsync(items); serviceMock.VerifyAll(); }
}

#pragma warning restore STXFORMAT004, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005