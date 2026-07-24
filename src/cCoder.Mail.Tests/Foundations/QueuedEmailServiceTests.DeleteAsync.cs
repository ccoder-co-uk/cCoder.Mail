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


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 9, appId: 7);

        queuedEmailBrokerMock.Setup(expression: x => x.GetAllQueuedEmails(ignoreFilters: true))
            .Returns(value: new[] { ToExternalQueuedEmail(item: queuedEmail) }.AsQueryable());

        queuedEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_delete"));

        queuedEmailBrokerMock
            .Setup(expression: x => x.DeleteAllQueuedEmailSendFailuresAsync(deletedEmailSendFailure: It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()))
            .Returns(value: ValueTask.CompletedTask);

        queuedEmailBrokerMock
            .Setup(expression: x =>
                x.DeleteQueuedEmailAsync(
deletedQueuedEmail: It.Is<cCoder.Data.Models.Mail.QueuedEmail>(match: candidate =>
                        candidate.Id == queuedEmail.Id
                        && candidate.AppId == queuedEmail.AppId
                        && candidate.Subject == queuedEmail.Subject
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await queuedEmailService.DeleteAsync(queuedEmailId: 9);

        // Then
        queuedEmailBrokerMock.Verify(expression: x => x.GetAllQueuedEmails(ignoreFilters: true), times: Times.Once);

        queuedEmailBrokerMock.Verify(
expression: x =>
                x.DeleteQueuedEmailAsync(
deletedQueuedEmail: It.Is<cCoder.Data.Models.Mail.QueuedEmail>(match: candidate =>
                        candidate.Id == queuedEmail.Id
                        && candidate.AppId == queuedEmail.AppId
                        && candidate.Subject == queuedEmail.Subject
                    )
                ),
times: Times.Once
        );

        queuedEmailBrokerMock.Verify(
expression: x => x.DeleteAllQueuedEmailSendFailuresAsync(
deletedEmailSendFailure: It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()
            ),
times: Times.Once
        );

        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 9, appId: 7);

        queuedEmailBrokerMock.Setup(expression: x => x.GetAllQueuedEmails(ignoreFilters: true))
            .Returns(value: new[] { ToExternalQueuedEmail(item: queuedEmail) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        queuedEmailBrokerMock
            .Setup(expression: x => x.DeleteAllQueuedEmailSendFailuresAsync(deletedEmailSendFailure: It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Func<Task> action = async () => await queuedEmailService.DeleteAsync(queuedEmailId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        queuedEmailBrokerMock.Verify(expression: x => x.GetAllQueuedEmails(ignoreFilters: true), times: Times.Once);
        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}