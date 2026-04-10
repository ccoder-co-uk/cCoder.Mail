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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 0, appId: 7);

        cCoder.Data.Models.Mail.MailServer submitted = null;

        mailServerBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "MailServer_create"));

        mailServerBrokerMock
            .Setup(x =>
                x.AddMailServerAsync(
                    It.Is<cCoder.Data.Models.Mail.MailServer>(candidate => !ReferenceEquals(candidate, mailServer))
                )
            )
            .Callback<cCoder.Data.Models.Mail.MailServer>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Mail.MailServer value) => value);

        // When
        MailServer result = await mailServerService.AddAsync(mailServer);

        // Then
        result.Should().NotBeSameAs(mailServer);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(mailServer, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(mailServer, options => options.Excluding(candidate => candidate.Id));

        mailServerBrokerMock.Verify(
            x =>
                x.AddMailServerAsync(
                    It.Is<cCoder.Data.Models.Mail.MailServer>(candidate => !ReferenceEquals(candidate, mailServer))
                ),
            Times.Once
        );
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "MailServer_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.AddAsync(mailServer);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









