// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009

using FluentAssertions;
using cCoder.Mail.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    [Fact]
    public void ShouldReturnMigrationState()
    {
        mailConfigurationBrokerMock
            .Setup(expression: broker => broker.GetMailConfiguration())
            .Returns(value: new MailConfiguration { IsMigrating = true });

        bool result = mailSendingService.IsMigrationInProgress();

        result.Should().BeTrue();
        mailConfigurationBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogDispatch()
    {
        // Given
        loggerMock
            .Setup(expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()));

        // When
        mailSendingService.LogDispatch(count: 3);

        // Then
        loggerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogSummary()
    {
        // Given
        loggerMock
            .Setup(expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()));

        // When
        mailSendingService.LogSummary(count: 3, success: 2, failures: 1);

        // Then
        loggerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogSendError()
    {
        // Given
        Exception exception = new(message: "send failed");
        loggerMock
            .Setup(expression: broker => broker.LogError(
                exception,
                exception.Message,
                It.IsAny<object[]>()));

        // When
        mailSendingService.LogError(exception: exception);

        // Then
        loggerMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009