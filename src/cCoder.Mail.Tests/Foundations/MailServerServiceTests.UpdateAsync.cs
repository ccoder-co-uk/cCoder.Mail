using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailServerServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(appId: 7);

        cCoder.Data.Models.Mail.MailServer submitted = null;

        mailServerBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "MailServer_update"));

        mailServerBrokerMock
            .Setup(x => x.UpdateMailServerAsync(It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Callback<cCoder.Data.Models.Mail.MailServer>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Mail.MailServer value) => value);

        // When
        MailServer result = await mailServerService.UpdateAsync(mailServer);

        // Then
        result.Should().BeSameAs(mailServer);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(mailServer);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(mailServer);
        result.Should().BeEquivalentTo(mailServer);
        mailServerBrokerMock.Verify(
            x => x.UpdateMailServerAsync(It.IsAny<cCoder.Data.Models.Mail.MailServer>()),
            Times.Once
        );
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "MailServer_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.UpdateAsync(mailServer);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









