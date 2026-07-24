// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Mail.Models;
using cCoder.Mail.Models.Exceptions;
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
    public async Task ShouldDeleteEachEmailWhenUserHasDeletePrivilegeForDeleteAllAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();

        queuedEmailServiceMock.Setup(expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: true))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await queuedEmailProcessingService.DeleteAllQueuedEmailAsync(deletedQueuedEmail: [email]);

        // Then
        queuedEmailServiceMock.Verify(expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: true), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAnEmailIsUnauthorizedForDeleteAllAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();

        queuedEmailServiceMock
            .Setup(expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: true))
            .ThrowsAsync(exception: new MailServiceException(
innerException: new SecurityException(message: "Access Denied!")));

        // When
        Func<Task> act = async () => await queuedEmailProcessingService.DeleteAllQueuedEmailAsync(deletedQueuedEmail: [email]);

        // Then

        await act.Should()
            .ThrowAsync<cCoder.Mail.Models.Exceptions.MailServiceException>()
            .WithMessage(expectedWildcardPattern: "The mail service failed.");

        queuedEmailServiceMock.Verify(
expression: x => x.DeleteAsync(iQueuedEmailId: email.Id, checkPrivileges: true),
times: Times.Once);

        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}