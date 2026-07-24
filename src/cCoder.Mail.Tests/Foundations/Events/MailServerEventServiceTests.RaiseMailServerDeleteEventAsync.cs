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

public partial class MailServerEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseMailServerDeleteEventAsync()
    {
        // Given
        MailServer entity = new();
        EventMessage<cCoder.Data.Models.Mail.MailServer> actualMessage = null;

        mailServerEventBrokerMock
            .Setup(expression: x => x.RaiseMailServerDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.MailServer>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.MailServer>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        mailServerEventBrokerMock.Verify(
expression: x => x.RaiseMailServerDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Mail.MailServer>>()),
times: Times.Once
        );

        mailServerEventBrokerMock.VerifyNoOtherCalls();
    }

}