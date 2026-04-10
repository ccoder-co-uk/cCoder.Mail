using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class SentEmailEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseSentEmailDeleteEventAsync()
    {
        // Given
        SentEmail entity = new();
        EventMessage<cCoder.Data.Models.Mail.SentEmail> actualMessage = null;

        sentEmailEventBrokerMock
            .Setup(x => x.RaiseSentEmailDeleteEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.SentEmail>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.SentEmail>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        sentEmailEventBrokerMock.Verify(
            x => x.RaiseSentEmailDeleteEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.SentEmail>>()),
            Times.Once
        );
        sentEmailEventBrokerMock.VerifyNoOtherCalls();
    }

}










