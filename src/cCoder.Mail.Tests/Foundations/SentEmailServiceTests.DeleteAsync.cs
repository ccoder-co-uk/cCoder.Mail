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

public partial class SentEmailServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 9, appId: 7);

        sentEmailBrokerMock.Setup(expression: x => x.GetAllSentEmails(ignoreFilters: true))
            .Returns(value: new[] { ToExternalSentEmail(item: sentEmail) }.AsQueryable());

        sentEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_delete"));

        sentEmailBrokerMock
            .Setup(expression: x =>
                x.DeleteSentEmailAsync(
deletedSentEmail: It.Is<cCoder.Data.Models.Mail.SentEmail>(match: candidate =>
                        candidate.Id == sentEmail.Id
                        && candidate.AppId == sentEmail.AppId
                        && candidate.Subject == sentEmail.Subject
                    )
                )
            )
            .ReturnsAsync(value: 1);

        // When
        await sentEmailService.DeleteAsync(sentEmailId: 9);

        // Then
        sentEmailBrokerMock.Verify(expression: x => x.GetAllSentEmails(ignoreFilters: true), times: Times.Once);

        sentEmailBrokerMock.Verify(
expression: x =>
                x.DeleteSentEmailAsync(
deletedSentEmail: It.Is<cCoder.Data.Models.Mail.SentEmail>(match: candidate =>
                        candidate.Id == sentEmail.Id
                        && candidate.AppId == sentEmail.AppId
                        && candidate.Subject == sentEmail.Subject
                    )
                ),
times: Times.Once
        );

        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 9, appId: 7);

        sentEmailBrokerMock.Setup(expression: x => x.GetAllSentEmails(ignoreFilters: true))
            .Returns(value: new[] { ToExternalSentEmail(item: sentEmail) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.DeleteAsync(sentEmailId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        sentEmailBrokerMock.Verify(expression: x => x.GetAllSentEmails(ignoreFilters: true), times: Times.Once);
        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}