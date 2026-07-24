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

public partial class SentEmailEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseSentEmailAddEventAsync()
    {
        // Given
        SentEmail entity = new();
        EventMessage<cCoder.Data.Models.Mail.SentEmail> actualMessage = null;

        sentEmailEventBrokerMock
            .Setup(expression: x => x.RaiseSentEmailAddEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.SentEmail>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.SentEmail>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailAddEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        sentEmailEventBrokerMock.Verify(
expression: x => x.RaiseSentEmailAddEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.SentEmail>>()),
times: Times.Once
        );

        sentEmailEventBrokerMock.VerifyNoOtherCalls();
    }

}