using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 7);

        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmails(false)).Returns(new[] { ToExternalQueuedEmail(queuedEmail) }.AsQueryable());

        // When
        QueuedEmail result = queuedEmailService.Get(7);

        // Then
        result.Should().BeEquivalentTo(queuedEmail);
        queuedEmailBrokerMock.Verify(x => x.GetAllQueuedEmails(false), Times.Once);
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}










