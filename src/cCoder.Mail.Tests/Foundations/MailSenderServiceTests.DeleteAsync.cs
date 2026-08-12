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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty, appId: 7);

        mailSenderBrokerMock.Setup(expression: x => x.GetAllMailSendersIgnoringFilters())
            .Returns(value: new[] { ToExternalMailSender(item: mailSender) }.AsQueryable());

        mailSenderBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailSender_delete"));

        mailSenderBrokerMock
            .Setup(expression: x =>
                x.DeleteMailSenderAsync(
deletedMailSender: It.Is<cCoder.Data.Models.Mail.MailSender>(match: candidate =>
                        candidate.Id == mailSender.Id
                        && candidate.AppId == mailSender.AppId
                        && candidate.Name == mailSender.Name
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await mailSenderService.DeleteAsync(mailSenderId: Guid.Empty);

        // Then
        mailSenderBrokerMock.Verify(expression: x => x.GetAllMailSendersIgnoringFilters(), times: Times.Once);

        mailSenderBrokerMock.Verify(
expression: x =>
                x.DeleteMailSenderAsync(
deletedMailSender: It.Is<cCoder.Data.Models.Mail.MailSender>(match: candidate =>
                        candidate.Id == mailSender.Id
                        && candidate.AppId == mailSender.AppId
                        && candidate.Name == mailSender.Name
                    )
                ),
times: Times.Once
        );

        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender(id: Guid.Empty, appId: 7);

        mailSenderBrokerMock.Setup(expression: x => x.GetAllMailSendersIgnoringFilters())
            .Returns(value: new[] { ToExternalMailSender(item: mailSender) }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailSenderService.DeleteAsync(mailSenderId: Guid.Empty);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailSenderBrokerMock.Verify(expression: x => x.GetAllMailSendersIgnoringFilters(), times: Times.Once);
        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005