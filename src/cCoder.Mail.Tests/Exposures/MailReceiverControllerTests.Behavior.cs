// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public void ShouldReturnMetadataWhenGetMetadataIsRequested()
    {
        IActionResult result = controller.GetMetadata();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnExtendedMetadataWhenGetMetadataIsExtended()
    {
        controller.Request.QueryString = new QueryString(value: "?extend=true");

        IActionResult result = controller.GetMetadata();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnMailReceiverWhenGetFindsRequestedMailReceiver()
    {
        MailReceiver mailReceiver = new() { Id = Guid.Empty };
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceiver(false))
            .Returns(value: new[] { mailReceiver }.AsQueryable());

        IActionResult result = controller.Get(key: mailReceiver.Id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedMailReceiver()
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceiver(false))
            .Returns(value: Array.Empty<MailReceiver>().AsQueryable());

        IActionResult result = controller.Get(key: Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newMailReceiver: new MailReceiver());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailReceiver: new MailReceiver());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchCannotFindMailReceiverAsync()
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetMailReceiver(iMailReceiverId: Guid.Empty))
            .Returns(value: null);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailReceiver: new Delta<MailReceiver>());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldUpdateMailReceiverEventWhenPatchFindsCalendarAsync()
    {
        MailReceiver mailReceiver = new() { Id = Guid.Empty };
        mailReceiverManagerMock.Setup(expression: service => service.GetMailReceiver(iMailReceiverId: Guid.Empty))
            .Returns(value: mailReceiver);
        mailReceiverManagerMock.Setup(expression: service => service.UpdateMailReceiverAsync(mailReceiver))
            .ReturnsAsync(value: mailReceiver);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailReceiver: new Delta<MailReceiver>());

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        mailReceiverManagerMock.Setup(expression: service => service.DeleteAsync(iMailReceiverId: Guid.Empty))
            .ReturnsAsync(value: 1);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<NoContentResult>();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005