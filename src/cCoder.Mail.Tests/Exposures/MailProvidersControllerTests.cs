// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class MailProvidersControllerTests
{
    private readonly Mock<IMailProviderCatalog> catalogMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly MailProvidersController controller;

    public MailProvidersControllerTests() => controller = new(catalogMock.Object, loggerMock.Object);

    [Fact]
    public void GetShouldCombineSendersAndReceivers()
    {
        catalogMock.Setup(x => x.GetSenders()).Returns([new MailProviderSummary { Name = "sender" }]);
        catalogMock.Setup(x => x.GetReceivers()).Returns([new MailProviderSummary { Name = "receiver" }]);
        ((MailProviderSummary[])((OkObjectResult)controller.Get()).Value).Should().HaveCount(2);
    }

    [Fact]
    public void GetSendersShouldReturnSenders()
    {
        MailProviderSummary[] providers = [new()]; catalogMock.Setup(x => x.GetSenders()).Returns(providers);
        ((OkObjectResult)controller.GetSenders()).Value.Should().BeSameAs(providers);
    }

    [Fact]
    public void GetReceiversShouldReturnReceivers()
    {
        MailProviderSummary[] providers = [new()]; catalogMock.Setup(x => x.GetReceivers()).Returns(providers);
        ((OkObjectResult)controller.GetReceivers()).Value.Should().BeSameAs(providers);
    }

    [Fact]
    public void GetShouldMapAndLogException()
    {
        var exception = new InvalidOperationException("failed");
        catalogMock.Setup(x => x.GetSenders()).Throws(exception); catalogMock.Setup(x => x.GetReceivers()).Throws(exception);
        IActionResult result = controller.Get();
        ((ObjectResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Fact]
    public void GetSendersShouldMapAndLogException()
    {
        var exception = new InvalidOperationException("failed");
        catalogMock.Setup(x => x.GetSenders()).Throws(exception);
        IActionResult result = controller.GetSenders();
        ((ObjectResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Fact]
    public void GetReceiversShouldMapAndLogException()
    {
        var exception = new InvalidOperationException("failed");
        catalogMock.Setup(x => x.GetReceivers()).Throws(exception);
        IActionResult result = controller.GetReceivers();
        ((ObjectResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005