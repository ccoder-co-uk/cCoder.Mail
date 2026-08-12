// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateAsync()
    {
        sentEmailServiceMock.Setup(x => x.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await sentEmailProcessingService.DeleteByAppIdAsync(7);
        sentEmailServiceMock.VerifyAll();
    }

    [Fact]
    public async Task AddOrUpdateSentEmailResultShouldAddNewEmailAsync()
    {
        SentEmail email = CreateRandomSentEmail(); email.Id = 0;
        sentEmailServiceMock.Setup(x => x.AddSentEmailAsync(email)).ReturnsAsync(email);
        IEnumerable<Result<SentEmail>> results = await sentEmailProcessingService.AddOrUpdateSentEmailResult([email]);
        results.Single().Success.Should().BeTrue(); results.Single().Message.Should().Be("Added Successfully");
    }

    [Fact]
    public async Task AddOrUpdateSentEmailResultShouldUpdateExistingEmailAsync()
    {
        SentEmail email = CreateRandomSentEmail();
        sentEmailServiceMock.Setup(x => x.GetAllSentEmail(true)).Returns(new[] { email }.AsQueryable());
        sentEmailServiceMock.Setup(x => x.UpdateSentEmailAsync(email)).ReturnsAsync(email);
        IEnumerable<Result<SentEmail>> results = await sentEmailProcessingService.AddOrUpdateSentEmailResult([email]);
        results.Single().Success.Should().BeTrue(); results.Single().Message.Should().Be("Updated Successfully");
    }

    [Fact]
    public async Task AddOrUpdateSentEmailResultShouldReportIndividualFailureAsync()
    {
        SentEmail email = CreateRandomSentEmail(); email.Id = 0;
        sentEmailServiceMock.Setup(x => x.AddSentEmailAsync(email)).ThrowsAsync(new InvalidOperationException("failed"));
        IEnumerable<Result<SentEmail>> results = await sentEmailProcessingService.AddOrUpdateSentEmailResult([email]);
        results.Single().Success.Should().BeFalse(); results.Single().Message.Should().Be("failed");
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005