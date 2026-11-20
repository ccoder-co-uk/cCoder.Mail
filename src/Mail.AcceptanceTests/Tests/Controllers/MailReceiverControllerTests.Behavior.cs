// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Linq;
#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public partial class MailReceiverControllerTests
{
    [Fact]
    public void ShouldReturnMailReceiverWhenGetFindsRequestedMailReceiver()
    {
        MailReceiver mailReceiver = new() { Id = Guid.Empty };
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceivers(false))
            .Returns(value: new[] { mailReceiver }.AsQueryable());

        IActionResult result = controller.Get(key: mailReceiver.Id);

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedMailReceiver()
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceivers(false))
            .Returns(value: Array.Empty<MailReceiver>().AsQueryable());

        IActionResult result = controller.Get(key: Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("Requested mail resource was not found.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void ShouldReturnMailReceiversWhenGetAllSucceeds()
    {
        mailReceiverManagerMock.Setup(service => service.GetAllMailReceivers(false))
            .Returns(Array.Empty<MailReceiver>().AsQueryable());

        IActionResult result = controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnCreatedMailReceiverWhenPostSucceedsAsync()
    {
        MailReceiver receiver = new();
        mailReceiverManagerMock.Setup(service => service.AddMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        IActionResult result = await controller.Post(receiver);

        result.Should().BeAssignableTo<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status201Created);
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnUpdatedMailReceiverWhenPutSucceedsAsync()
    {
        MailReceiver receiver = new();
        mailReceiverManagerMock.Setup(service => service.UpdateMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        IActionResult result = await controller.Put(Guid.Empty, receiver);

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newMailReceiver: new MailReceiver());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("The mail request is invalid.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailReceiver: new MailReceiver());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("The mail request is invalid.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        mailReceiverManagerMock.Setup(expression: service => service.DeleteAsync(iMailReceiverId: Guid.Empty))
            .ReturnsAsync(value: 1);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<NoContentResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005