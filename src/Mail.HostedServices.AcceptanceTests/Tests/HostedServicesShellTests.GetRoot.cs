// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace HostedServices.AcceptanceTests.Tests;

public sealed partial class HostedServicesShellTests
{
    [Fact]
    public async Task GetRoot_ReturnsHostedServicesReport()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/");

        // Then

        content.Should()
            .Contain(expected: "cCoder.Mail Hosted Services");

        content.Should()
            .Contain(expected: "MailSenderHostedService");

        content.Should()
            .Contain(expected: "MailReceiverHostedService");

        content.Should()
            .Contain(expected: "app_add");

        content.Should()
            .Contain(expected: "app_update");

        content.Should()
            .Contain(expected: "app_delete");
    }
}