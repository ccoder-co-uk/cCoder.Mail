// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailReceivingProcessingServiceTests
{
    private readonly Mock<IMailReceivingService> receivingServiceMock = new();
    private readonly Mock<IMailConfigurationExposure> configurationMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly MailReceivingProcessingService service;

    public MailReceivingProcessingServiceTests() =>
        service = new(receivingServiceMock.Object, configurationMock.Object, loggerMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMigrationInProgressShouldReturnConfigurationValue(bool migrating)
    {
        configurationMock.Setup(x => x.GetMailConfiguration()).Returns(new MailConfiguration { IsMigrating = migrating });
        service.IsMigrationInProgress().Should().Be(migrating);
    }

    [Fact]
    public void LogErrorShouldLogException()
    {
        var exception = new InvalidOperationException("failed");
        service.LogError(exception);
        loggerMock.Verify(x => x.LogError(exception, "failed"), Times.Once);
    }

    [Fact]
    public async Task ReceiveMailboxReceiveRequestAsyncShouldDelegateAsync()
    {
        var request = new MailboxReceiveRequest();
        ReceivedEmail[] emails = [new() { Id = 1 }];
        receivingServiceMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(request, CancellationToken.None)).ReturnsAsync(emails);
        (await service.ReceiveMailboxReceiveRequestAsync(request)).Should().BeSameAs(emails);
    }

    [Fact]
    public async Task ReceiveTopAsyncShouldDelegateAsync()
    {
        Guid receiverId = Guid.NewGuid();
        ReceivedEmail[] emails = [new() { Id = 1 }];
        receivingServiceMock.Setup(x => x.ReceiveTopAsync(receiverId, 5, CancellationToken.None)).ReturnsAsync(emails);
        (await service.ReceiveTopAsync(receiverId, 5)).Should().BeSameAs(emails);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005