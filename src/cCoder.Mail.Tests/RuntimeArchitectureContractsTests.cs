// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Providers.Exposures.MailClients;
using FluentAssertions;
using Xunit;

namespace cCoder.Mail.Tests;

public sealed partial class RuntimeArchitectureContractsTests
{
    [Theory]
    [InlineData(typeof(LoggingBroker), "IUtilityBroker")]
    [InlineData(typeof(MailClientFactory), "ICompositionExposure")]
    public void RuntimeType_WhenMarkerIsResolved_UsesContractsAssembly(
        Type runtimeType,
        string markerInterfaceName)
    {
        // Given
        Type markedRuntimeType = runtimeType;

        // When
        Type markerInterface = markedRuntimeType
            .GetInterfaces()
            .Single(predicate: interfaceType =>
                interfaceType.Name == markerInterfaceName);

        string markerAssemblyName = markerInterface.Assembly
            .GetName()
            .Name;

        // Then
        markerAssemblyName.Should()
            .Be(expected: "cCoder.CodeAnalysis.Contracts");
    }

    [Fact]
    public void MailRuntime_WhenOutputIsBuilt_ContainsContractsWithoutAnalyzerRuntime()
    {
        // Given
        string runtimeOutputDirectory = Path.GetDirectoryName(
            path: typeof(LoggingBroker).Assembly.Location);

        string contractsAssemblyPath = Path.Combine(
            path1: runtimeOutputDirectory,
            path2: "cCoder.CodeAnalysis.Contracts.dll");

        string analyzerAssemblyPath = Path.Combine(
            path1: runtimeOutputDirectory,
            path2: "cCoder.CodeAnalysis.dll");

        // When
        bool contractsAssemblyExists = File.Exists(path: contractsAssemblyPath);
        bool analyzerAssemblyExists = File.Exists(path: analyzerAssemblyPath);

        // Then
        contractsAssemblyExists.Should()
            .BeTrue();

        analyzerAssemblyExists.Should()
            .BeFalse();
    }
}