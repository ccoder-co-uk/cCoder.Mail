// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServers(ignoreFilters: true))
            .Returns(value: new[] { ToExternalMailServer(item: mailServer) }.AsQueryable());

        mailServerBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_delete"));

        mailServerBrokerMock
            .Setup(expression: x =>
                x.DeleteMailServerAsync(
entity: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await mailServerService.DeleteAsync(id: 9);

        // Then
        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServers(ignoreFilters: true), times: Times.Once);

        mailServerBrokerMock.Verify(
expression: x =>
                x.DeleteMailServerAsync(
entity: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate =>
                        candidate.Id == mailServer.Id
                        && candidate.AppId == mailServer.AppId
                        && candidate.Name == mailServer.Name
                    )
                ),
times: Times.Once
        );

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 9, appId: 7);

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServers(ignoreFilters: true))
            .Returns(value: new[] { ToExternalMailServer(item: mailServer) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.DeleteAsync(id: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServers(ignoreFilters: true), times: Times.Once);
        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}