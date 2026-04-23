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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(appId: 7);

        cCoder.Data.Models.Mail.SentEmail submitted = null;

        sentEmailBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "SentEmail_update"));

        sentEmailBrokerMock
            .Setup(x => x.UpdateSentEmailAsync(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()))
            .Callback<cCoder.Data.Models.Mail.SentEmail>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Mail.SentEmail value) => value);

        // When
        SentEmail result = await sentEmailService.UpdateAsync(sentEmail);

        // Then
        result.Should().BeSameAs(sentEmail);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(sentEmail);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(sentEmail);
        result.Should().BeEquivalentTo(sentEmail);
        sentEmailBrokerMock.Verify(x => x.UpdateSentEmailAsync(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.Once);
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "SentEmail_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.UpdateAsync(sentEmail);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









