// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Mail.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures.EventHandlers;

#pragma warning disable STXFORMAT005

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void MailEventStartup_WhenInspected_IsExposedFromIEventHubExtensions()
    {
        // Given
        Type mailAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type eventHubExtensionsType = mailAssemblyMarker.Assembly
            .GetType(name: "cCoder.Mail.IEventHubExtensions");

        // Then
        eventHubExtensionsType
            .Should()
            .NotBeNull();

        eventHubExtensionsType!
            .GetMethods()
            .Where(predicate: method =>
                method.IsPublic &&
                method.IsStatic &&
                method.Name == "ListenToMailEvents")
            .Should()
            .ContainSingle()
            .Which
            .GetParameters()[0]
            .ParameterType
            .Should()
            .Be(typeof(IEventHub));
    }

    [Fact]
    public void MailEventListening_WhenInspected_HasNoIntermediateListenerTypes()
    {
        // Given
        Type mailAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type[] listenerInfrastructure = mailAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Name is
                    "MailEventHandlers" or
                    "IMailEventHandlers" or
                    "EventHandlerService" or
                    "IEventHandlerService" or
                    "EventHubBroker" or
                    "IEventHubBroker")
            .ToArray();

        // Then
        listenerInfrastructure
            .Should()
            .BeEmpty(
                because: "mail listeners should be composed directly on IEventHub");
    }

    [Fact]
    public void MailEvents_WhenStarted_RegisterExpectedBusinessBoundaries()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        (string EventName, Type ServiceType)[] expectedRegistrations =
        [
            ("app_add", typeof(IAppAggregationService)),
            ("app_update", typeof(IAppAggregationService)),
            ("app_delete", typeof(IAppAggregationService))
        ];

        // When
        IEventHub result = eventHubMock.Object.ListenToMailEvents();

        // Then
        result
            .Should()
            .BeSameAs(eventHubMock.Object);

        eventHubMock.Invocations
            .Select(selector: invocation =>
                (
                    EventName: (string)invocation.Arguments[0],
                    ServiceType: invocation.Method.GetGenericArguments()[1]
                ))
            .Should()
            .BeEquivalentTo(expectation: expectedRegistrations);
    }

    [Fact]
    public async Task AppEvents_WhenRaised_InvokeExpectedAggregationOperationsAsync()
    {
        // Given
        const int appId = 17;
        App app = new() { Id = appId };
        Mock<IEventHub> eventHubMock = new();
        Mock<IAppAggregationService> serviceMock = new(MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.AddAppAsync(app))
            .Returns(value: ValueTask.CompletedTask);

        serviceMock
            .Setup(expression: service => service.UpdateAppAsync(app))
            .Returns(value: ValueTask.CompletedTask);

        serviceMock
            .Setup(expression: service => service.DeleteAsync(appId))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToMailEvents();

        // When
        await GetHandler(eventHubMock: eventHubMock, eventName: "app_add")(
            arg1: serviceMock.Object,
            arg2: app);

        await GetHandler(eventHubMock: eventHubMock, eventName: "app_update")(
            arg1: serviceMock.Object,
            arg2: app);

        await GetHandler(eventHubMock: eventHubMock, eventName: "app_delete")(
            arg1: serviceMock.Object,
            arg2: app);

        // Then
        serviceMock.VerifyAll();
    }

    private static Func<IAppAggregationService, App, ValueTask> GetHandler(
        Mock<IEventHub> eventHubMock,
        string eventName) =>
        (Func<IAppAggregationService, App, ValueTask>)eventHubMock.Invocations
            .Single(predicate: invocation =>
                invocation.Arguments[0] as string == eventName)
            .Arguments[1];
}

#pragma warning restore STXFORMAT005