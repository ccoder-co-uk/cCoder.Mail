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
        content.Should().Contain("/tools/company-logo.png");
        content.Should().Contain("mail-logo");
        content.Should().Contain("Sign in required");
        content.Should().Contain("mail-login-gate");
        content.Should().Contain("mail-workbench");
        content.Should().Contain("<nav class=\"mail-section-tabs\"");
        content.Should().MatchRegex("</nav>\\s*<section class=\"mail-panel\">");
        content.Should().Contain("mail-server-grid");
        content.Should().Contain("queued-email-grid");
        content.Should().Contain("sent-email-grid");
        content.Should().Contain("received-email-grid");
        content.Should().Contain("Queued Emails");
        content.Should().Contain("Sent Emails");
        content.Should().Contain("Received Emails");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("/tools/styles.css?v=mail-grid-20260702-auth");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/api.js");

        // Then
        content.Should().Contain("mail-auth-changed");
        content.Should().Contain("isAuthenticated: function");
        content.Should().Contain("document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/grids.js");

        // Then
        content.Should().Contain("MailGrids");
        content.Should().Contain("MailApi.isAuthenticated()");
        content.Should().Contain("mail-auth-changed");
        content.Should().Contain("data-child-grid=\"QueuedEmail\"");
        content.Should().Contain("data-child-grid=\"SentEmail\"");
        content.Should().Contain("data-child-grid=\"EmailSendFailure\"");
        content.Should().Contain("loadMailServerDetails");
        content.Should().Contain("loadQueuedEmailDetails");
        content.Should().Contain("receiveEmails");
        content.Should().Contain("ReceivedEmail/Receive");
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
        content.Should().Contain(".mail-section-tabs button.active");
        content.Should().Contain(".mail-logo");
        content.Should().Contain("body.mail-shell:not(.is-authenticated) .mail-workbench");
        content.Should().Contain("body.mail-shell.is-authenticated .mail-login-gate");
        content.Should().Contain("border-radius: 4px 4px 0 0");
    }
}
