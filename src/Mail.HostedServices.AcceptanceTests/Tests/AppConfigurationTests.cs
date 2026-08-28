// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Mail.Models;
using Xunit;

namespace Mail.HostedServices.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void ShouldExposeEveryRequiredDomainConfiguration()
    {
        // Given
        const string typeName =
            "Mail.HostedServices.Models.AppConfiguration, Mail.HostedServices";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        Assert.NotNull(@object: configurationType);

        Assert.Equal(
            expected: typeof(CoreDataConfiguration),
            actual: configurationType.GetProperty(name: "CoreData")?.PropertyType);

        Assert.Equal(
            expected: typeof(MailConfiguration),
            actual: configurationType.GetProperty(name: "Mail")?.PropertyType);

        Assert.Equal(
            expected: typeof(EventingConfiguration),
            actual: configurationType.GetProperty(name: "Eventing")?.PropertyType);
    }
}