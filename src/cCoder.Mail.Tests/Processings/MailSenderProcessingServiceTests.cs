// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailSenderProcessingServiceTests
{
    private readonly Mock<IMailSenderService> serviceMock = new();
    private readonly MailSenderProcessingService service;

    public MailSenderProcessingServiceTests() => service = new(serviceMock.Object);

    [Fact]
    public void GetMailSenderShouldDelegate()
    {
        Guid id = Guid.NewGuid(); var sender = new MailSender { Id = id };
        serviceMock.Setup(x => x.GetMailSender(id)).Returns(sender);
        service.GetMailSender(id).Should().BeSameAs(sender);
    }

    [Fact]
    public void GetAllMailSenderShouldDelegate()
    {
        IQueryable<MailSender> senders = Array.Empty<MailSender>().AsQueryable();
        serviceMock.Setup(x => x.GetAllMailSender(true)).Returns(senders);
        service.GetAllMailSender(true).Should().BeSameAs(senders);
    }

    [Fact]
    public async Task AddMailSenderAsyncShouldDelegateAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid() }; serviceMock.Setup(x => x.AddMailSenderAsync(sender)).ReturnsAsync(sender);
        (await service.AddMailSenderAsync(sender)).Should().BeSameAs(sender);
    }

    [Fact]
    public async Task UpdateMailSenderAsyncShouldDelegateAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid() }; serviceMock.Setup(x => x.UpdateMailSenderAsync(sender)).ReturnsAsync(sender);
        (await service.UpdateMailSenderAsync(sender)).Should().BeSameAs(sender);
    }

    [Fact]
    public async Task DeleteAsyncShouldDelegateAsync()
    {
        Guid id = Guid.NewGuid(); serviceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(1);
        (await service.DeleteAsync(id)).Should().Be(1);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateAsync()
    {
        serviceMock.Setup(x => x.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await service.DeleteByAppIdAsync(7); serviceMock.VerifyAll();
    }

    [Fact]
    public async Task DeleteAllMailSenderAsyncShouldDelegateAsync()
    {
        MailSender[] senders = [new() { Id = Guid.NewGuid() }];
        serviceMock.Setup(x => x.DeleteAllMailSenderAsync(senders)).Returns(ValueTask.CompletedTask);
        await service.DeleteAllMailSenderAsync(senders); serviceMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005