// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{

    [Fact]
    public async Task ShouldAuthorizeUpdateAndClearFailuresForRetryAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();

        currentUser = TestUsers.WithPrivilege(
            privilege: "queuedemail_update",
            appId: email.AppId);

        queuedEmailServiceMock
            .Setup(expression: service => service.GetQueuedEmail(iQueuedEmailId: email.Id))
            .Returns(value: email);

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: currentUser);

        queuedEmailServiceMock
            .Setup(expression: service => service.RetryAsync(queuedEmailId: email.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await queuedEmailProcessingService.RetryAsync(queuedEmailId: email.Id);

        // Then
        queuedEmailServiceMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005