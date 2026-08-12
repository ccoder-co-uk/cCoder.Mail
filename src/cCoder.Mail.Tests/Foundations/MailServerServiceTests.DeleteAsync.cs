// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

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

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServersIgnoringFilters())
            .Returns(value: new[] { ToExternalMailServer(item: mailServer) }.AsQueryable());

        mailServerBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailServer_delete"));

        mailServerBrokerMock
            .Setup(expression: x =>
                x.DeleteMailServerAsync(
deletedMailServer: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await mailServerService.DeleteAsync(mailServerId: 9);

        // Then
        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServersIgnoringFilters(), times: Times.Once);

        mailServerBrokerMock.Verify(
expression: x =>
                x.DeleteMailServerAsync(
deletedMailServer: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                ),
times: Times.Once
        );

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 9, appId: 7);

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServersIgnoringFilters())
            .Returns(value: new[] { ToExternalMailServer(item: mailServer) }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailServerService.DeleteAsync(mailServerId: 9);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServersIgnoringFilters(), times: Times.Once);
        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005