using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SentEmailServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 7);

        sentEmailBrokerMock.Setup(x => x.GetAllSentEmails(false)).Returns(new[] { ToExternalSentEmail(sentEmail) }.AsQueryable());

        // When
        SentEmail result = sentEmailService.Get(7);

        // Then
        result.Should().BeEquivalentTo(sentEmail);
        sentEmailBrokerMock.Verify(x => x.GetAllSentEmails(false), Times.Once);
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}










