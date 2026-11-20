// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Foundations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed class MailProvidersControllerTests
{
    [Theory]
    [InlineData("Get")]
    [InlineData("GetSenders")]
    [InlineData("GetReceivers")]
    public void ActionWhenCatalogSucceedsShouldReturn200AndLogInformation(string action)
    {
        Mock<IMailProviderCatalogService> catalog = new();
        Mock<ILoggingBroker> logging = new();
        catalog.Setup(service => service.GetSenders()).Returns([]);
        catalog.Setup(service => service.GetReceivers()).Returns([]);
        MailProvidersController controller = new(catalog.Object, logging.Object);

        IActionResult result = Invoke(controller, action);

        result.Should().BeOfType<OkObjectResult>();
        logging.Verify(broker => broker.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
        logging.Verify(broker => broker.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        logging.Verify(broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
    }

    [Theory]
    [InlineData("Get")]
    [InlineData("GetSenders")]
    [InlineData("GetReceivers")]
    public void ActionWhenCatalogRejectsRequestShouldReturn400AndWarn(string action)
    {
        Mock<IMailProviderCatalogService> catalog = new();
        Mock<ILoggingBroker> logging = new();
        MailValidationException exception = new(innerException: new Exception("Invalid provider request."));
        catalog.Setup(service => service.GetSenders()).Throws(exception);
        catalog.Setup(service => service.GetReceivers()).Throws(exception);
        MailProvidersController controller = new(catalog.Object, logging.Object);

        IActionResult result = Invoke(controller, action);

        result.Should().BeOfType<BadRequestObjectResult>();
        logging.Verify(broker => broker.LogWarning(exception.Message, It.IsAny<object[]>()), Times.Once);
        logging.Verify(broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        logging.Verify(broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
    }

    [Theory]
    [InlineData("Get")]
    [InlineData("GetSenders")]
    [InlineData("GetReceivers")]
    public void ActionWhenCatalogFailsShouldReturn500AndLogError(string action)
    {
        Mock<IMailProviderCatalogService> catalog = new();
        Mock<ILoggingBroker> logging = new();
        InvalidOperationException exception = new("Provider failed.");
        catalog.Setup(service => service.GetSenders()).Throws(exception);
        catalog.Setup(service => service.GetReceivers()).Throws(exception);
        MailProvidersController controller = new(catalog.Object, logging.Object);

        IActionResult result = Invoke(controller, action);

        result.Should().BeAssignableTo<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        logging.Verify(broker => broker.LogError(exception, "Controller request failed.", It.IsAny<object[]>()), Times.Once);
        logging.Verify(broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        logging.Verify(broker => broker.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
    }

    static IActionResult Invoke(MailProvidersController controller, string action) =>
        action switch
        {
            "Get" => controller.Get(),
            "GetSenders" => controller.GetSenders(),
            "GetReceivers" => controller.GetReceivers(),
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };
}
