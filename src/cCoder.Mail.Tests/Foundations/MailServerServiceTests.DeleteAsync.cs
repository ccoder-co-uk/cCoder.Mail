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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 9, appId: 7);

        mailServerBrokerMock.Setup(x => x.GetAllMailServers(true)).Returns(new[] { ToExternalMailServer(mailServer) }.AsQueryable());

        mailServerBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "MailServer_delete"));
        mailServerBrokerMock
            .Setup(x =>
                x.DeleteMailServerAsync(
                    It.Is<cCoder.Data.Models.Mail.MailServer>(candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                )
            )
            .ReturnsAsync(1);

        // When
        await mailServerService.DeleteAsync(9);

        // Then
        mailServerBrokerMock.Verify(x => x.GetAllMailServers(true), Times.Once);
        mailServerBrokerMock.Verify(
            x =>
                x.DeleteMailServerAsync(
                    It.Is<cCoder.Data.Models.Mail.MailServer>(candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                ),
            Times.Once
        );
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 9, appId: 7);

        mailServerBrokerMock.Setup(x => x.GetAllMailServers(true)).Returns(new[] { ToExternalMailServer(mailServer) }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "MailServer_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        mailServerBrokerMock.Verify(x => x.GetAllMailServers(true), Times.Once);
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "MailServer_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









