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

public partial class MailReceiverServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver(id: Guid.Empty, appId: 7);

        cCoder.Data.Models.Mail.MailReceiver submitted = null;

        mailReceiverBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailReceiver_create"));

        mailReceiverBrokerMock
            .Setup(expression: x =>
                x.AddMailReceiverAsync(
newMailReceiver: It.Is<cCoder.Data.Models.Mail.MailReceiver>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailReceiver))
                )
            )
            .Callback<cCoder.Data.Models.Mail.MailReceiver>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.MailReceiver value) => value);

        // When
        MailReceiver result = await mailReceiverService.AddMailReceiverAsync(newMailReceiver: mailReceiver);

        // Then

        result.Should()
            .NotBeSameAs(unexpected: mailReceiver);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: mailReceiver);

        result.Should()
            .BeSameAs(expected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: mailReceiver, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: mailReceiver, config: options => options.Excluding(expression: candidate => candidate.Id));

        mailReceiverBrokerMock.Verify(
expression: x =>
                x.AddMailReceiverAsync(
newMailReceiver: It.Is<cCoder.Data.Models.Mail.MailReceiver>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailReceiver))
                ),
times: Times.Once
        );

        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver(id: Guid.Empty, appId: 7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailReceiverService.AddMailReceiverAsync(newMailReceiver: mailReceiver);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005