// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
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

        missingVariables.Should()
            .BeEmpty(
because: "the mail receive integration needs configured Microsoft Graph and acceptance database settings. "
            + $"Missing variables: {string.Join(separator: ", ", value: missingVariables)}");

        await using IntegrationApplication application = await StartApplicationAsync(settings: settings);

        // When
        ReceivedEmail[] receivedEmails = await ReceiveTopEmailsAsync(client: application.Client, count: 1);

        // Then

        receivedEmails.Should()
            .ContainSingle();

        receivedEmails[0].Subject.Should()
            .NotBeNullOrWhiteSpace();

        receivedEmails[0].ReceivedOn.Should()
            .NotBe(unexpected: default);
    }
}