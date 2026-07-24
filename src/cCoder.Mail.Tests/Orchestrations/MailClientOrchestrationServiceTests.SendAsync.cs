// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using FizzWare.NBuilder;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailClientOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToSendingServiceWhenSendAsync()
    {
        // Given
        QueuedEmail email = Builder<QueuedEmail>.CreateNew()
            .Build();

        CancellationToken cancellationToken = new();

        mailSendingServiceMock
            .Setup(expression: service => service.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailClientOrchestrationService.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then
        mailSendingServiceMock.Verify(expression: service => service.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailSendingServiceMock.VerifyNoOtherCalls();
        mailReceivingServiceMock.VerifyNoOtherCalls();
    }
}