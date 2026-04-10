using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations.Events;

public partial class QueuedEmailEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseQueuedEmailUpdateEventAsync()
    {
        // Given
        QueuedEmail entity = new();
        EventMessage<cCoder.Data.Models.Mail.QueuedEmail> actualMessage = null;

        queuedEmailEventBrokerMock
            .Setup(x => x.RaiseQueuedEmailUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>()))
            .Callback<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        queuedEmailEventBrokerMock.Verify(
            x => x.RaiseQueuedEmailUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Mail.QueuedEmail>>()),
            Times.Once
        );
        queuedEmailEventBrokerMock.VerifyNoOtherCalls();
    }

}










