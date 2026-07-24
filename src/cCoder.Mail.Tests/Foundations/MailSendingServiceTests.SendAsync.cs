// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using FizzWare.NBuilder;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenSendAsync()
    {
        // Given
        QueuedEmail email = Builder<QueuedEmail>.CreateNew()
            .Build();

        CancellationToken cancellationToken = new();

        mailSenderClientBrokerMock
            .Setup(expression: broker => broker.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailSendingService.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken);

        // Then
        mailSenderClientBrokerMock.Verify(expression: broker => broker.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailSenderClientBrokerMock.VerifyNoOtherCalls();
    }
}