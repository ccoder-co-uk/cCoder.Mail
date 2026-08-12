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

public partial class ReceivedEmailServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 0, appId: 7);

        cCoder.Data.Models.Mail.ReceivedEmail submitted = null;

        receivedEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "ReceivedEmail_create"));

        receivedEmailBrokerMock
            .Setup(expression: x =>
                x.AddReceivedEmailAsync(
newReceivedEmail: It.Is<cCoder.Data.Models.Mail.ReceivedEmail>(match: candidate => !ReferenceEquals(objA: candidate, objB: receivedEmail))
                )
            )
            .Callback<cCoder.Data.Models.Mail.ReceivedEmail>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.ReceivedEmail value) => value);

        // When
        ReceivedEmail result = await receivedEmailService.AddReceivedEmailAsync(newReceivedEmail: receivedEmail);

        // Then

        result.Should()
            .NotBeSameAs(unexpected: receivedEmail);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: receivedEmail);

        result.Should()
            .BeSameAs(expected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: receivedEmail, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: receivedEmail, config: options => options.Excluding(expression: candidate => candidate.Id));

        receivedEmailBrokerMock.Verify(
expression: x =>
                x.AddReceivedEmailAsync(
newReceivedEmail: It.Is<cCoder.Data.Models.Mail.ReceivedEmail>(match: candidate => !ReferenceEquals(objA: candidate, objB: receivedEmail))
                ),
times: Times.Once
        );

        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 0, appId: 7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await receivedEmailService.AddReceivedEmailAsync(newReceivedEmail: receivedEmail);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005