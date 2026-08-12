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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 9, appId: 7);

        receivedEmailBrokerMock.Setup(expression: x => x.GetAllReceivedEmailsIgnoringFilters())
            .Returns(value: new[] { ToExternalReceivedEmail(item: receivedEmail) }.AsQueryable());

        receivedEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "ReceivedEmail_delete"));

        receivedEmailBrokerMock
            .Setup(expression: x =>
                x.DeleteReceivedEmailAsync(
deletedReceivedEmail: It.Is<cCoder.Data.Models.Mail.ReceivedEmail>(match: candidate =>
                        candidate.Id == receivedEmail.Id
                        && candidate.AppId == receivedEmail.AppId
                        && candidate.Subject == receivedEmail.Subject
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await receivedEmailService.DeleteAsync(receivedEmailId: 9);

        // Then
        receivedEmailBrokerMock.Verify(expression: x => x.GetAllReceivedEmailsIgnoringFilters(), times: Times.Once);

        receivedEmailBrokerMock.Verify(
expression: x =>
                x.DeleteReceivedEmailAsync(
deletedReceivedEmail: It.Is<cCoder.Data.Models.Mail.ReceivedEmail>(match: candidate =>
                        candidate.Id == receivedEmail.Id
                        && candidate.AppId == receivedEmail.AppId
                        && candidate.Subject == receivedEmail.Subject
                    )
                ),
times: Times.Once
        );

        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail(id: 9, appId: 7);

        receivedEmailBrokerMock.Setup(expression: x => x.GetAllReceivedEmailsIgnoringFilters())
            .Returns(value: new[] { ToExternalReceivedEmail(item: receivedEmail) }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await receivedEmailService.DeleteAsync(receivedEmailId: 9);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        receivedEmailBrokerMock.Verify(expression: x => x.GetAllReceivedEmailsIgnoringFilters(), times: Times.Once);
        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005