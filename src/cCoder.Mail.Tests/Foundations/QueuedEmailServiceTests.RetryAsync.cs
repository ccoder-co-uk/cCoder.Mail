// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{

    [Fact]
    public async Task ShouldDeleteOnlyFailureRowsForRetryAsync()
    {
        // Given
        const int queuedEmailId = 42;

        queuedEmailBrokerMock
            .Setup(expression: broker => broker.DeleteAllQueuedEmailSendFailuresAsync(
                emailId: queuedEmailId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await queuedEmailService.RetryAsync(queuedEmailId: queuedEmailId);

        // Then
        queuedEmailBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005