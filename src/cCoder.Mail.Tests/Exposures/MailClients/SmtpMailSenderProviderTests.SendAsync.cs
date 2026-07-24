// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class SmtpMailSenderProviderTests
{
    [Fact]
    public async Task ShouldDelegateToServiceWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        smtpMailSenderServiceMock
            .Setup(expression: service => service.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await smtpMailSenderProvider.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then

        smtpMailSenderProvider.ProviderName.Should()
            .Be(expected: MailProviderNames.Smtp);

        smtpMailSenderServiceMock.Verify(expression: service => service.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        smtpMailSenderServiceMock.VerifyNoOtherCalls();
    }
}