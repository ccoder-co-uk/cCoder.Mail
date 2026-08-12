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
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEmailWhenUserHasDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

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
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
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
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
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
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        QueuedEmail email = CreateRandomQueuedEmail();
        currentUser = TestUsers.WithoutPrivileges();

        queuedEmailServiceMock.Setup(expression: x => x.GetAllQueuedEmail(ignoreFilters: true))
            .Returns(value: new[] { email }.AsQueryable());

        // When
        Func<Task> act = async () => await queuedEmailProcessingService.DeleteAsync(queuedEmailId: email.Id);

        // Then

        await act.Should()
            .ThrowAsync<cCoder.Mail.Providers.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        queuedEmailServiceMock.Verify(expression: x => x.GetAllQueuedEmail(ignoreFilters: true), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005