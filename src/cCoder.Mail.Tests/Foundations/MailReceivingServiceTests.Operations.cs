// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009

using FluentAssertions;
using cCoder.Mail.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceivingServiceTests
{
    [Fact]
    public void ShouldReturnMigrationState()
    {
        mailConfigurationBrokerMock
            .Setup(expression: broker => broker.GetMailConfiguration())
            .Returns(value: new MailConfiguration { IsMigrating = true });

        bool result = mailReceivingService.IsMigrationInProgress();

        result.Should().BeTrue();
        mailConfigurationBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogReceiveError()
    {
        // Given
        Exception exception = new(message: "receive failed");
        loggerMock
            .Setup(expression: broker => broker.LogError(
                exception,
                exception.Message,
                It.IsAny<object[]>()));

        // When
        mailReceivingService.LogError(exception: exception);

        // Then
        loggerMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009