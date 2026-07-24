// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace Mail.IntegrationTests.Tests;

public sealed partial class MailDeliveryTests
{
    [Fact]
    public async Task SendAndReceiveAsync_SendsQueuedMailAndReceivesItFromMailbox()
    {
        // Given
        IntegrationSettings settings = ReadSettings();

        string[] missingVariables = settings.MissingVariables();

        missingVariables.Should()
            .BeEmpty(
because: "the mail end-to-end integration needs configured Microsoft Graph and acceptance database settings. "
            + $"Missing variables: {string.Join(separator: ", ", value: missingVariables)}");

        await using IntegrationApplication application = await StartApplicationAsync(settings: settings);

        string unique = Guid.NewGuid()
            .ToString(format: "N");

        string subject = $"Integration Test Send {unique}";
        string content = $"cCoder Mail integration content {unique}";
        DateTimeOffset receiveFrom = DateTimeOffset.UtcNow.AddMinutes(minutes: -2);

        // When

        QueuedEmail queuedEmail = await QueueEmailAsync(
client: application.Client,
appId: application.AppId,
mailSenderId: application.MailSenderId,
subject: subject,
content: content,
to: settings.To);

        await DispatchQueuedMailAsync(services: application.Factory.Services);
        IReadOnlyList<SentEmail> sentEmails = await GetSentEmailsAsync(client: application.Client, subject: subject);

        sentEmails.Should()
            .ContainSingle(predicate: email => email.Subject == subject && email.To == settings.To);

        ReceivedEmail receivedEmail = await ReceiveEmailAsync(
client: application.Client,
settings: settings,
subject: subject,
content: content,
from: receiveFrom);

        // Then

        queuedEmail.Subject.Should()
            .Be(expected: subject);

        receivedEmail.Subject.Should()
            .Be(expected: subject);

        receivedEmail.Content.Should()
            .Contain(expected: content);
    }
}