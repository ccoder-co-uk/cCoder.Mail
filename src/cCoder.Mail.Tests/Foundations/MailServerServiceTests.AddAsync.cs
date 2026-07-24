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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 0, appId: 7);

        cCoder.Data.Models.Mail.MailServer submitted = null;

        mailServerBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_create"));

        mailServerBrokerMock
            .Setup(expression: x =>
                x.AddMailServerAsync(
newMailServer: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailServer))
                )
            )
            .Callback<cCoder.Data.Models.Mail.MailServer>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.MailServer value) => value);

        // When
        MailServer result = await mailServerService.AddMailServerAsync(newMailServer: mailServer);

        // Then

        result.Should()
            .BeSameAs(expected: mailServer);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: mailServer);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: mailServer, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: mailServer, config: options => options.Excluding(expression: candidate => candidate.Id));

        mailServerBrokerMock.Verify(
expression: x =>
                x.AddMailServerAsync(
newMailServer: It.Is<cCoder.Data.Models.Mail.MailServer>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailServer))
                ),
times: Times.Once
        );

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await mailServerService.AddMailServerAsync(newMailServer: mailServer);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "MailServer_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}