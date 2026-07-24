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
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEmailWhenUserHasDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        QueuedEmail email = CreateRandomQueuedEmail();
        DataUser actor = TestUsers.WithPrivilege(privilege: "queuedemail_delete", appId: email.AppId);
        currentUser = actor;

        queuedEmailServiceMock.Setup(expression: x => x.GetAllQueuedEmail(ignoreFilters: true))
            .Returns(value: new[] { email }.AsQueryable());

        queuedEmailServiceMock.Setup(expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: false))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await queuedEmailProcessingService.DeleteAsync(queuedEmailId: email.Id);

        // Then
        queuedEmailServiceMock.Verify(expression: x => x.GetAllQueuedEmail(ignoreFilters: true), times: Times.Once);
        queuedEmailServiceMock.Verify(expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: false), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: email.AppId, privilege: "queuedemail_delete"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenEmailDoesNotExistForDeleteAsync()
    {
        // Given
        queuedEmailServiceMock
            .Setup(expression: x => x.GetAllQueuedEmail(ignoreFilters: true))
            .Returns(value: Array.Empty<QueuedEmail>()
            .AsQueryable());

        // When
        Func<Task> act = async () => await queuedEmailProcessingService.DeleteAsync(queuedEmailId: 99);

        // Then

        await act.Should()
            .ThrowAsync<cCoder.Mail.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        queuedEmailServiceMock.Verify(expression: x => x.GetAllQueuedEmail(ignoreFilters: true), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        QueuedEmail email = CreateRandomQueuedEmail();
        currentUser = TestUsers.WithoutPrivileges();

        queuedEmailServiceMock.Setup(expression: x => x.GetAllQueuedEmail(ignoreFilters: true))
            .Returns(value: new[] { email }.AsQueryable());

        // When
        Func<Task> act = async () => await queuedEmailProcessingService.DeleteAsync(queuedEmailId: email.Id);

        // Then

        await act.Should()
            .ThrowAsync<cCoder.Mail.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        queuedEmailServiceMock.Verify(expression: x => x.GetAllQueuedEmail(ignoreFilters: true), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: email.AppId, privilege: "queuedemail_delete"), times: Times.Once);
    }
}