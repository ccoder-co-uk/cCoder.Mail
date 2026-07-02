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
        string content = await GetOkContentAsync("/");

        // Then
        content.Should().Contain("cCoder.Mail Hosted Services");
        content.Should().Contain("MailSenderHostedService");
        content.Should().Contain("MailReceiverHostedService");
        content.Should().Contain("app_add");
        content.Should().Contain("app_update");
        content.Should().Contain("app_delete");
    }
}
