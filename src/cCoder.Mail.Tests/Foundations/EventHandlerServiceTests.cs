// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.CMS;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Foundations.Events;

public sealed partial class EventHandlerServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings => new()
    {
        { new MailValidationException(new Exception()), typeof(MailValidationException) },
        { new MailDependencyException(new Exception()), typeof(MailDependencyException) },
        { new ArgumentException("invalid"), typeof(MailValidationException) },
        { new InvalidOperationException("failed"), typeof(MailServiceException) },
    };

    [Fact]
    public void ListenToAllEventsShouldRegisterAppHandlers()
    {
        Mock<IEventHubBroker> brokerMock = new(); var service = new EventHandlerService(brokerMock.Object);
        service.ListenToAllEvents();
        brokerMock.Verify(x => x.ListenToEvent<App, IAppAggregationService>("app_add", It.IsAny<Func<IAppAggregationService, App, ValueTask>>()), Times.Once);
        brokerMock.Verify(x => x.ListenToEvent<App, IAppAggregationService>("app_update", It.IsAny<Func<IAppAggregationService, App, ValueTask>>()), Times.Once);
        brokerMock.Verify(x => x.ListenToEvent<App, IAppAggregationService>("app_delete", It.IsAny<Func<IAppAggregationService, App, ValueTask>>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ListenToAllEventsShouldMapException(Exception exception, Type expectedType)
    {
        Mock<IEventHubBroker> brokerMock = new();
        brokerMock.Setup(x => x.ListenToEvent<App, IAppAggregationService>(It.IsAny<string>(), It.IsAny<Func<IAppAggregationService, App, ValueTask>>())).Throws(exception);
        var service = new EventHandlerService(brokerMock.Object);
        Action action = () => service.ListenToAllEvents();
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005