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

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateAsync()
    {
        mailServerServiceMock.Setup(x => x.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await mailServerProcessingService.DeleteByAppIdAsync(7);
        mailServerServiceMock.VerifyAll();
    }

    [Fact]
    public async Task AddOrUpdateMailServerResultShouldAddNewServerAsync()
    {
        MailServer server = CreateRandomMailServer(); server.Id = 0;
        mailServerServiceMock.Setup(x => x.AddMailServerAsync(server, false)).ReturnsAsync(server);
        IEnumerable<Result<MailServer>> results = await ((global::cCoder.Mail.Services.Processings.IMailServerProcessingService)mailServerProcessingService).AddOrUpdateMailServerResult([server]);
        results.Single().Success.Should().BeTrue(); results.Single().Message.Should().Be("Added Successfully");
    }

    [Fact]
    public async Task AddOrUpdateMailServerResultShouldUpdateExistingServerAsync()
    {
        MailServer server = CreateRandomMailServer();
        mailServerServiceMock.Setup(x => x.GetAllMailServer(true)).Returns(new[] { server }.AsQueryable());
        mailServerServiceMock.Setup(x => x.UpdateMailServerAsync(server)).ReturnsAsync(server);
        IEnumerable<Result<MailServer>> results = await ((global::cCoder.Mail.Services.Processings.IMailServerProcessingService)mailServerProcessingService).AddOrUpdateMailServerResult([server]);
        results.Single().Success.Should().BeTrue(); results.Single().Message.Should().Be("Updated Successfully");
    }

    [Fact]
    public async Task AddOrUpdateMailServerResultShouldReportIndividualFailureAsync()
    {
        MailServer server = CreateRandomMailServer(); server.Id = 0;
        mailServerServiceMock.Setup(x => x.AddMailServerAsync(server, false)).ThrowsAsync(new InvalidOperationException("failed"));
        IEnumerable<Result<MailServer>> results = await ((global::cCoder.Mail.Services.Processings.IMailServerProcessingService)mailServerProcessingService).AddOrUpdateMailServerResult([server]);
        results.Single().Success.Should().BeFalse(); results.Single().Message.Should().Be("failed");
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005