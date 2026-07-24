// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace HostedServices.AcceptanceTests.Tests;

public sealed partial class HostedServicesShellTests
{
    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/Health");

        // Then

        content.Should()
            .Be(expected: "Healthy");
    }
}