using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace Mail.IntegrationTests.Tests;

public sealed partial class MailDeliveryTests
{
    [Fact]
    public async Task ReceiveTopAsync_ReturnsTopEmailFromConfiguredMailbox()
    {
        // Given
        IntegrationSettings settings = ReadSettings();
        string[] missingVariables = settings.MissingVariables();
        missingVariables.Should().BeEmpty(
            "the mail receive integration needs configured Microsoft Graph and acceptance database settings. "
            + $"Missing variables: {string.Join(", ", missingVariables)}");

        await using IntegrationApplication application = await StartApplicationAsync(settings);

        // When
        ReceivedEmail[] receivedEmails = await ReceiveTopEmailsAsync(application.Client, 1);

        // Then
        receivedEmails.Should().ContainSingle();
        receivedEmails[0].Subject.Should().NotBeNullOrWhiteSpace();
        receivedEmails[0].ReceivedOn.Should().NotBeNull();
    }
}
