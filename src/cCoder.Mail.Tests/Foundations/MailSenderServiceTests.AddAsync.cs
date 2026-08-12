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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty, appId: 7);

        cCoder.Data.Models.Mail.MailSender submitted = null;

        mailSenderBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailSender_create"));

        mailSenderBrokerMock
            .Setup(expression: x =>
                x.AddMailSenderAsync(
newMailSender: It.Is<cCoder.Data.Models.Mail.MailSender>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailSender))
                )
            )
            .Callback<cCoder.Data.Models.Mail.MailSender>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.MailSender value) => value);

        // When
        MailSender result = await mailSenderService.AddMailSenderAsync(newMailSender: mailSender);

        // Then

        result.Should()
            .NotBeSameAs(unexpected: mailSender);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: mailSender);

        result.Should()
            .BeSameAs(expected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: mailSender, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: mailSender, config: options => options.Excluding(expression: candidate => candidate.Id));

        mailSenderBrokerMock.Verify(
expression: x =>
                x.AddMailSenderAsync(
newMailSender: It.Is<cCoder.Data.Models.Mail.MailSender>(match: candidate => !ReferenceEquals(objA: candidate, objB: mailSender))
                ),
times: Times.Once
        );

        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty, appId: 7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailSenderService.AddMailSenderAsync(newMailSender: mailSender);

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