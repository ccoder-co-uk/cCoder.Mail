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

        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmails(true)).Returns(new[] { ToExternalQueuedEmail(queuedEmail) }.AsQueryable());

        queuedEmailBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "QueuedEmail_delete"));
        queuedEmailBrokerMock
            .Setup(x => x.DeleteAllQueuedEmailSendFailuresAsync(It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()))
            .Returns(ValueTask.CompletedTask);
        queuedEmailBrokerMock
            .Setup(x =>
                x.DeleteQueuedEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.QueuedEmail>(candidate =>
                        candidate.Id == queuedEmail.Id
                        && candidate.AppId == queuedEmail.AppId
                        && candidate.Subject == queuedEmail.Subject
                    )
                )
            )
            .ReturnsAsync(1);

        // When
        await queuedEmailService.DeleteAsync(9);

        // Then
        queuedEmailBrokerMock.Verify(x => x.GetAllQueuedEmails(true), Times.Once);
        queuedEmailBrokerMock.Verify(
            x =>
                x.DeleteQueuedEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.QueuedEmail>(candidate =>
                        candidate.Id == queuedEmail.Id
                        && candidate.AppId == queuedEmail.AppId
                        && candidate.Subject == queuedEmail.Subject
                    )
                ),
            Times.Once
        );
        queuedEmailBrokerMock.Verify(
            x => x.DeleteAllQueuedEmailSendFailuresAsync(
                It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()
            ),
            Times.Once
        );
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "QueuedEmail_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 9, appId: 7);

        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmails(true)).Returns(new[] { ToExternalQueuedEmail(queuedEmail) }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "QueuedEmail_delete"))
            .Throws(new SecurityException("Access Denied!"));
        queuedEmailBrokerMock
            .Setup(x => x.DeleteAllQueuedEmailSendFailuresAsync(It.IsAny<IEnumerable<cCoder.Data.Models.Mail.EmailSendFailure>>()))
            .Returns(ValueTask.CompletedTask);

        // When
        Func<Task> action = async () => await queuedEmailService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        queuedEmailBrokerMock.Verify(x => x.GetAllQueuedEmails(true), Times.Once);
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "QueuedEmail_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









