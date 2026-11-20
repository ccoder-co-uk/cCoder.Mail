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

public partial class MailSenderControllerTests
{
    [Fact]
    public void ShouldReturnMailSenderWhenGetFindsRequestedMailSender()
    {
        MailSender mailSender = new() { Id = Guid.Empty };
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSenders(false))
            .Returns(value: new[] { mailSender }.AsQueryable());

        IActionResult result = controller.Get(key: mailSender.Id);

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedMailSender()
    {
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSenders(false))
            .Returns(value: Array.Empty<MailSender>().AsQueryable());

        IActionResult result = controller.Get(key: Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("Requested mail resource was not found.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void ShouldReturnMailSendersWhenGetAllSucceeds()
    {
        mailSenderManagerMock.Setup(service => service.GetAllMailSenders(false))
            .Returns(Array.Empty<MailSender>().AsQueryable());

        IActionResult result = controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnCreatedMailSenderWhenPostSucceedsAsync()
    {
        MailSender sender = new();
        mailSenderManagerMock.Setup(service => service.AddMailSenderAsync(sender)).ReturnsAsync(sender);

        IActionResult result = await controller.Post(sender);

        result.Should().BeAssignableTo<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status201Created);
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnUpdatedMailSenderWhenPutSucceedsAsync()
    {
        MailSender sender = new();
        mailSenderManagerMock.Setup(service => service.UpdateMailSenderAsync(sender)).ReturnsAsync(sender);

        IActionResult result = await controller.Put(Guid.Empty, sender);

        result.Should().BeOfType<OkObjectResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newMailSender: new MailSender());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("The mail request is invalid.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailSender: new MailSender());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
        loggingBrokerMock.Verify(x => x.LogWarning("The mail request is invalid.", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        mailSenderManagerMock.Setup(expression: service => service.DeleteAsync(iMailSenderId: Guid.Empty))
            .ReturnsAsync(value: 1);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<NoContentResult>();
        loggingBrokerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005