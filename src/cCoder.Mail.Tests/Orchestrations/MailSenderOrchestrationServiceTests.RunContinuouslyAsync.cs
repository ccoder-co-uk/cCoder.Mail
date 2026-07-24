// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailSenderOrchestrationServiceTests
{
    [Fact]
    public async Task RunContinuouslyAsyncShouldCompleteWhenCancellationIsRequested()
    {
        // Given
        using CancellationTokenSource cancellationTokenSource = new();

        mailSendingProcessingServiceMock
            .Setup(expression: service => service.IsMigrationInProgress())
            .Returns(value: false);

        cancellationTokenSource.Cancel();

        // When
        Func<Task> runContinuouslyAsync = () =>
            mailSenderOrchestrationService.RunContinuouslyAsync(
                cancellationToken: cancellationTokenSource.Token);

        // Then
        await runContinuouslyAsync.Should()
            .NotThrowAsync();
    }
}