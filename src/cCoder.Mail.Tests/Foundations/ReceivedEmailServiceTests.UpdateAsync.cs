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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 7);

        cCoder.Data.Models.Mail.ReceivedEmail submitted = null;

        receivedEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "ReceivedEmail_update"));

        receivedEmailBrokerMock
            .Setup(expression: x => x.UpdateReceivedEmailAsync(updatedReceivedEmail: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()))
            .Callback<cCoder.Data.Models.Mail.ReceivedEmail>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.ReceivedEmail value) => value);

        // When
        ReceivedEmail result = await receivedEmailService.UpdateReceivedEmailAsync(updatedReceivedEmail: receivedEmail);

        // Then

        result.Should()
            .NotBeSameAs(unexpected: receivedEmail);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: receivedEmail);

        result.Should()
            .BeSameAs(expected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: receivedEmail);

        result.Should()
            .BeEquivalentTo(expectation: receivedEmail);

        receivedEmailBrokerMock.Verify(expression: x => x.UpdateReceivedEmailAsync(updatedReceivedEmail: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.Once);
        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await receivedEmailService.UpdateReceivedEmailAsync(updatedReceivedEmail: receivedEmail);

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