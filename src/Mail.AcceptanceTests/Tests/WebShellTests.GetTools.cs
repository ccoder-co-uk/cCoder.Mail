// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        string content = await GetOkContentAsync(path: "/tools/index.html");

        // Then

        content.Should()
            .Contain(expected: "Mail Management");

        content.Should()
            .Contain(expected: "/tools/company-logo.png");

        content.Should()
            .Contain(expected: "mail-logo");

        content.Should()
            .Contain(expected: "Sign in required");

        content.Should()
            .Contain(expected: "mail-login-gate");

        content.Should()
            .Contain(expected: "mail-workbench");

        content.Should()
            .Contain(expected: "<nav class=\"mail-section-tabs\"");

        content.Should()
            .MatchRegex(regularExpression: "</nav>\\s*<section class=\"mail-panel\">");

        content.Should()
            .Contain(expected: "mail-server-grid");

        content.Should()
            .Contain(expected: "mail-sender-grid");

        content.Should()
            .Contain(expected: "mail-receiver-grid");

        content.Should()
            .Contain(expected: "queued-email-grid");

        content.Should()
            .Contain(expected: "sent-email-grid");

        content.Should()
            .Contain(expected: "received-email-grid");

        content.Should()
            .Contain(expected: "Mail Senders");

        content.Should()
            .Contain(expected: "Mail Receivers");

        content.Should()
            .Contain(expected: "Queued Emails");

        content.Should()
            .Contain(expected: "Sent Emails");

        content.Should()
            .Contain(expected: "Received Emails");

        content.Should()
            .Contain(expected: "/tools/api.js");

        content.Should()
            .Contain(expected: "/tools/grids.js");

        content.Should()
            .Contain(expected: "/tools/styles.css?v=mail-grid-20260702-providers");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/api.js");

        // Then

        content.Should()
            .Contain(expected: "mail-auth-changed");

        content.Should()
            .Contain(expected: "isAuthenticated: function");

        content.Should()
            .Contain(expected: "document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/grids.js");

        // Then

        content.Should()
            .Contain(expected: "MailGrids");

        content.Should()
            .Contain(expected: "MailApi.isAuthenticated()");

        content.Should()
            .Contain(expected: "mail-auth-changed");

        content.Should()
            .Contain(expected: "data-child-grid=\"QueuedEmail\"");

        content.Should()
            .Contain(expected: "data-child-grid=\"SentEmail\"");

        content.Should()
            .Contain(expected: "data-child-grid=\"EmailSendFailure\"");

        content.Should()
            .Contain(expected: "loadProviders");

        content.Should()
            .Contain(expected: "loadMailSenderDetails");

        content.Should()
            .Contain(expected: "loadQueuedEmailDetails");

        content.Should()
            .Contain(expected: "receiveEmails");

        content.Should()
            .Contain(expected: "ReceivedEmail/Receive");

        content.Should()
            .Contain(expected: "MailSenderId eq");

        content.Should()
            .Contain(expected: "$expand=");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsGridShellStyles()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/styles.css");

        // Then

        content.Should()
            .Contain(expected: ".mail-table");

        content.Should()
            .Contain(expected: ".mail-detail");

        content.Should()
            .Contain(expected: ".mail-tab-panel");

        content.Should()
            .Contain(expected: ".mail-section-tabs");

        content.Should()
            .Contain(expected: ".mail-section-tabs button.active");

        content.Should()
            .Contain(expected: ".mail-logo");

        content.Should()
            .Contain(expected: "body.mail-shell:not(.is-authenticated) .mail-workbench");

        content.Should()
            .Contain(expected: "body.mail-shell.is-authenticated .mail-login-gate");

        content.Should()
            .Contain(expected: "border-radius: 4px 4px 0 0");
    }
}