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

public partial class MailSenderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty);

        cCoder.Data.Models.Mail.MailSender submitted = null;

        mailSenderBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailSender_update"));

        mailSenderBrokerMock
            .Setup(expression: x => x.UpdateMailSenderAsync(updatedMailSender: It.IsAny<cCoder.Data.Models.Mail.MailSender>()))
            .Callback<cCoder.Data.Models.Mail.MailSender>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.MailSender value) => value);

        // When
        MailSender result = await mailSenderService.UpdateMailSenderAsync(updatedMailSender: mailSender);

        // Then

        result.Should()
            .NotBeSameAs(unexpected: mailSender);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: mailSender);

        result.Should()
            .BeSameAs(expected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: mailSender);

        result.Should()
            .BeEquivalentTo(expectation: mailSender);

        mailSenderBrokerMock.Verify(
expression: x => x.UpdateMailSenderAsync(updatedMailSender: It.IsAny<cCoder.Data.Models.Mail.MailSender>()),
times: Times.Once
        );

        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailSenderService.UpdateMailSenderAsync(updatedMailSender: mailSender);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005