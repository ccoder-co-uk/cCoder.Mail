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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(appId: 7);

        cCoder.Data.Models.Mail.QueuedEmail submitted = null;

        queuedEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_update"));

        queuedEmailBrokerMock
            .Setup(expression: x => x.UpdateQueuedEmailAsync(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()))
            .Callback<cCoder.Data.Models.Mail.QueuedEmail>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.QueuedEmail value) => value);

        // When
        QueuedEmail result = await queuedEmailService.UpdateAsync(queuedEmail: queuedEmail);

        // Then

        result.Should()
            .BeSameAs(expected: queuedEmail);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: queuedEmail);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(
expectation: queuedEmail,
config: options => options.Excluding(expression: candidate => candidate.FailedSends)
        );

        result.Should()
            .BeEquivalentTo(expectation: queuedEmail);

        queuedEmailBrokerMock.Verify(
expression: x => x.UpdateQueuedEmailAsync(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()),
times: Times.Once
        );

        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await queuedEmailService.UpdateAsync(queuedEmail: queuedEmail);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "QueuedEmail_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}