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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();
        IQueryable<cCoder.Data.Models.Mail.SentEmail> sentEmails = new[] { ToExternalSentEmail(sentEmail) }.AsQueryable();

        sentEmailBrokerMock.Setup(x => x.GetAllSentEmails(false)).Returns(sentEmails);

        // When
        IQueryable<SentEmail> result = sentEmailService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(sentEmail);
        sentEmailBrokerMock.Verify(x => x.GetAllSentEmails(false), Times.Once);
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









