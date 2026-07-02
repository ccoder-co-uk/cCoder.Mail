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
        missingVariables.Should().BeEmpty(
            "the mail end-to-end integration needs configured Microsoft Graph and acceptance database settings. "
            + $"Missing variables: {string.Join(", ", missingVariables)}");

        await using IntegrationApplication application = await StartApplicationAsync(settings);
        string unique = Guid.NewGuid().ToString("N");
        string subject = $"Integration Test Send {unique}";
        string content = $"cCoder Mail integration content {unique}";
        DateTimeOffset receiveFrom = DateTimeOffset.UtcNow.AddMinutes(-2);

        // When
        QueuedEmail queuedEmail = await QueueEmailAsync(
            application.Client,
            application.AppId,
            application.MailSenderId,
            subject,
            content,
            settings.To);

        await DispatchQueuedMailAsync(application.Factory.Services);
        IReadOnlyList<SentEmail> sentEmails = await GetSentEmailsAsync(application.Client, subject);
        sentEmails.Should().ContainSingle(email => email.Subject == subject && email.To == settings.To);

        ReceivedEmail receivedEmail = await ReceiveEmailAsync(
            application.Client,
            settings,
            subject,
            content,
            receiveFrom);

        // Then
        queuedEmail.Subject.Should().Be(subject);
        receivedEmail.Subject.Should().Be(subject);
        receivedEmail.Content.Should().Contain(content);
    }
}
