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

public sealed partial class ReceivedEmailProcessingServiceTests
{
    private readonly Mock<IReceivedEmailService> foundationMock = new();
    private readonly ReceivedEmailProcessingService service;

    public ReceivedEmailProcessingServiceTests() => service = new(foundationMock.Object);

    [Fact]
    public void GetReceivedEmailShouldDelegate()
    {
        var email = new ReceivedEmail { Id = 1 }; foundationMock.Setup(x => x.GetReceivedEmail(1)).Returns(email);
        service.GetReceivedEmail(1).Should().BeSameAs(email);
    }

    [Fact]
    public void GetAllReceivedEmailShouldDelegate()
    {
        IQueryable<ReceivedEmail> emails = Array.Empty<ReceivedEmail>().AsQueryable(); foundationMock.Setup(x => x.GetAllReceivedEmail(true)).Returns(emails);
        service.GetAllReceivedEmail(true).Should().BeSameAs(emails);
    }

    [Fact]
    public async Task AddReceivedEmailAsyncShouldDelegateAsync()
    {
        var email = new ReceivedEmail { Id = 1 }; foundationMock.Setup(x => x.AddReceivedEmailAsync(email)).ReturnsAsync(email);
        (await service.AddReceivedEmailAsync(email)).Should().BeSameAs(email);
    }

    [Fact]
    public async Task UpdateReceivedEmailAsyncShouldDelegateAsync()
    {
        var email = new ReceivedEmail { Id = 1 }; foundationMock.Setup(x => x.UpdateReceivedEmailAsync(email)).ReturnsAsync(email);
        (await service.UpdateReceivedEmailAsync(email)).Should().BeSameAs(email);
    }

    [Fact]
    public async Task DeleteAsyncShouldDelegateAsync()
    {
        foundationMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(1);
        (await service.DeleteAsync(1)).Should().Be(1);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateAsync()
    {
        foundationMock.Setup(x => x.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await service.DeleteByAppIdAsync(7); foundationMock.VerifyAll();
    }

    [Fact]
    public async Task AddRangeReceivedEmailAsyncShouldDelegateAsync()
    {
        ReceivedEmail[] emails = [new() { Id = 1 }]; foundationMock.Setup(x => x.AddRangeReceivedEmailAsync(emails, CancellationToken.None)).Returns(ValueTask.CompletedTask);
        await service.AddRangeReceivedEmailAsync(emails); foundationMock.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExistsShouldReturnFoundationValue(bool exists)
    {
        Guid receiverId = Guid.NewGuid(); foundationMock.Setup(x => x.Exists(receiverId, "message")).Returns(exists);
        service.Exists(receiverId, "message").Should().Be(exists);
    }

    [Fact]
    public async Task DeleteAllReceivedEmailAsyncShouldDelegateAsync()
    {
        ReceivedEmail[] emails = [new() { Id = 1 }]; foundationMock.Setup(x => x.DeleteAllReceivedEmailAsync(emails)).Returns(ValueTask.CompletedTask);
        await service.DeleteAllReceivedEmailAsync(emails); foundationMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005