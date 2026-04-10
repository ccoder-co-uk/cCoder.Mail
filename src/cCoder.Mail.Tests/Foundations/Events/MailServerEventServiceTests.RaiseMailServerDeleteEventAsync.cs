using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
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
            .Setup(x => x.RaiseMailServerDeleteEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.MailServer>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.MailServer>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        mailServerEventBrokerMock.Verify(
            x => x.RaiseMailServerDeleteEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.MailServer>>()),
            Times.Once
        );
        mailServerEventBrokerMock.VerifyNoOtherCalls();
    }

}










