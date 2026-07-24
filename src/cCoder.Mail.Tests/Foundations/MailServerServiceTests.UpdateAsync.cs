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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 7);

        cCoder.Data.Models.Mail.MailServer submitted = null;

        mailServerBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_update"));

        mailServerBrokerMock
            .Setup(expression: x => x.UpdateMailServerAsync(updatedMailServer: It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Callback<cCoder.Data.Models.Mail.MailServer>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.MailServer value) => value);

        // When
        MailServer result = await mailServerService.UpdateMailServerAsync(updatedMailServer: mailServer);

        // Then

        result.Should()
            .BeSameAs(expected: mailServer);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: mailServer);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: mailServer);

        result.Should()
            .BeEquivalentTo(expectation: mailServer);

        mailServerBrokerMock.Verify(
expression: x => x.UpdateMailServerAsync(updatedMailServer: It.IsAny<cCoder.Data.Models.Mail.MailServer>()),
times: Times.Once
        );

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.UpdateMailServerAsync(updatedMailServer: mailServer);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}