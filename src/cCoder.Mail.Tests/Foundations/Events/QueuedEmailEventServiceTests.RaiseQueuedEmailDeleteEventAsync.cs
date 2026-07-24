// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class QueuedEmailEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseQueuedEmailDeleteEventAsync()
    {
        // Given
        QueuedEmail entity = new();
        EventMessage<cCoder.Data.Models.Mail.QueuedEmail> actualMessage = null;

        queuedEmailEventBrokerMock
            .Setup(expression: x => x.RaiseQueuedEmailDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        queuedEmailEventBrokerMock.Verify(
expression: x => x.RaiseQueuedEmailDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>()),
times: Times.Once
        );

        queuedEmailEventBrokerMock.VerifyNoOtherCalls();
    }

}