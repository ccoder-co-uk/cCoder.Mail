// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailSendingProcessingServiceTests
{
    private readonly Mock<IMailSendingService> sendingServiceMock = new();
    private readonly Mock<IMailConfigurationExposure> configurationMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly MailSendingProcessingService service;

    public MailSendingProcessingServiceTests() =>
        service = new(sendingServiceMock.Object, configurationMock.Object, loggerMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMigrationInProgressShouldReturnConfigurationValue(bool migrating)
    {
        configurationMock.Setup(x => x.GetMailConfiguration()).Returns(new MailConfiguration { IsMigrating = migrating });
        service.IsMigrationInProgress().Should().Be(migrating);
    }

    [Fact]
    public void LogDispatchShouldLogCount()
    {
        service.LogDispatch(3);
        loggerMock.Verify(x => x.LogInformation("Picked up a batch of {Count} emails.", 3), Times.Once);
    }

    [Fact]
    public void LogSummaryShouldLogCounts()
    {
        service.LogSummary(4, 3, 1);
        loggerMock.Verify(x => x.LogInformation(
            "{Count} SMTP requests made of which {Success} succeeded and {Failures} failed.",
            It.Is<object[]>(values => values.SequenceEqual(new object[] { 4, 3, 1 }))), Times.Once);
    }

    [Fact]
    public void LogErrorShouldLogException()
    {
        var exception = new InvalidOperationException("failed");
        service.LogError(exception);
        loggerMock.Verify(x => x.LogError(exception, "failed"), Times.Once);
    }

    [Fact]
    public async Task SendQueuedEmailAsyncShouldDelegateAsync()
    {
        var email = new QueuedEmail { Id = 1 };
        sendingServiceMock.Setup(x => x.SendQueuedEmailAsync(email, CancellationToken.None)).Returns(Task.CompletedTask);
        await service.SendQueuedEmailAsync(email);
        sendingServiceMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005