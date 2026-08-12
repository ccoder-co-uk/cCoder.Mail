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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver(id: Guid.Empty, appId: 7);

        mailReceiverBrokerMock.Setup(expression: x => x.GetAllMailReceiversIgnoringFilters())
            .Returns(value: new[] { ToExternalMailReceiver(item: mailReceiver) }.AsQueryable());

        mailReceiverBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateAuthorized(appId: (int?)7, privilege: "MailReceiver_delete"));

        mailReceiverBrokerMock
            .Setup(expression: x =>
                x.DeleteMailReceiverAsync(
deletedMailReceiver: It.Is<cCoder.Data.Models.Mail.MailReceiver>(match: candidate =>
                        candidate.Id == mailReceiver.Id
                        && candidate.AppId == mailReceiver.AppId
                        && candidate.Name == mailReceiver.Name
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await mailReceiverService.DeleteAsync(mailReceiverId: Guid.Empty);

        // Then
        mailReceiverBrokerMock.Verify(expression: x => x.GetAllMailReceiversIgnoringFilters(), times: Times.Once);

        mailReceiverBrokerMock.Verify(
expression: x =>
                x.DeleteMailReceiverAsync(
deletedMailReceiver: It.Is<cCoder.Data.Models.Mail.MailReceiver>(match: candidate =>
                        candidate.Id == mailReceiver.Id
                        && candidate.AppId == mailReceiver.AppId
                        && candidate.Name == mailReceiver.Name
                    )
                ),
times: Times.Once
        );

        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver(id: Guid.Empty, appId: 7);

        mailReceiverBrokerMock.Setup(expression: x => x.GetAllMailReceiversIgnoringFilters())
            .Returns(value: new[] { ToExternalMailReceiver(item: mailReceiver) }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: AuthorizationTestUsers.CreateUnauthorized());

        // When
        Func<Task> action = async () => await mailReceiverService.DeleteAsync(mailReceiverId: Guid.Empty);

        // Then

        await action.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        mailReceiverBrokerMock.Verify(expression: x => x.GetAllMailReceiversIgnoringFilters(), times: Times.Once);
        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005