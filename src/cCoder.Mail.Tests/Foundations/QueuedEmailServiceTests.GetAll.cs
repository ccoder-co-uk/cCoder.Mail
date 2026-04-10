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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail();
        IQueryable<cCoder.Data.Models.Mail.QueuedEmail> queuedEmails = new[] { ToExternalQueuedEmail(queuedEmail) }.AsQueryable();

        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmails(false)).Returns(queuedEmails);

        // When
        IQueryable<QueuedEmail> result = queuedEmailService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(queuedEmail);
        queuedEmailBrokerMock.Verify(x => x.GetAllQueuedEmails(false), Times.Once);
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









