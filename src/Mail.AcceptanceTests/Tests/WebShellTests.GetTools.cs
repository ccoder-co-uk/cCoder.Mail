using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetTools_ReturnsMailManagementShell()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/index.html");

        // Then
        content.Should().Contain("Mail Management");
        content.Should().Contain("mail-server-grid");
        content.Should().Contain("queued-email-grid");
        content.Should().Contain("sent-email-grid");
        content.Should().Contain("Queued Emails");
        content.Should().Contain("Sent Emails");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("/tools/styles.css");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/grids.js");

        // Then
        content.Should().Contain("MailGrids");
        content.Should().Contain("data-child-grid=\"QueuedEmail\"");
        content.Should().Contain("data-child-grid=\"SentEmail\"");
        content.Should().Contain("data-child-grid=\"EmailSendFailure\"");
        content.Should().Contain("loadMailServerDetails");
        content.Should().Contain("loadQueuedEmailDetails");
        content.Should().Contain("MailServerName eq");
        content.Should().Contain("$expand=");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsGridShellStyles()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/styles.css");

        // Then
        content.Should().Contain(".mail-table");
        content.Should().Contain(".mail-detail");
        content.Should().Contain(".mail-tab-panel");
        content.Should().Contain(".mail-section-tabs");
    }
}
