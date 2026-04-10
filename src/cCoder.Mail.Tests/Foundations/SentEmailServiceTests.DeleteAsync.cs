using System.Security;
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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 9, appId: 7);

        sentEmailBrokerMock.Setup(x => x.GetAllSentEmails(true)).Returns(new[] { ToExternalSentEmail(sentEmail) }.AsQueryable());

        sentEmailBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "SentEmail_delete"));
        sentEmailBrokerMock
            .Setup(x =>
                x.DeleteSentEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.SentEmail>(candidate =>
                        candidate.Id == sentEmail.Id
                        && candidate.AppId == sentEmail.AppId
                        && candidate.Subject == sentEmail.Subject
                    )
                )
            )
            .ReturnsAsync(1);

        // When
        await sentEmailService.DeleteAsync(9);

        // Then
        sentEmailBrokerMock.Verify(x => x.GetAllSentEmails(true), Times.Once);
        sentEmailBrokerMock.Verify(
            x =>
                x.DeleteSentEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.SentEmail>(candidate =>
                        candidate.Id == sentEmail.Id
                        && candidate.AppId == sentEmail.AppId
                        && candidate.Subject == sentEmail.Subject
                    )
                ),
            Times.Once
        );
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 9, appId: 7);

        sentEmailBrokerMock.Setup(x => x.GetAllSentEmails(true)).Returns(new[] { ToExternalSentEmail(sentEmail) }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "SentEmail_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        sentEmailBrokerMock.Verify(x => x.GetAllSentEmails(true), Times.Once);
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









