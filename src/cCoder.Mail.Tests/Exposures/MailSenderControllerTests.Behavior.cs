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

public partial class MailSenderControllerTests
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
    public void ShouldReturnMailSenderWhenGetFindsRequestedMailSender()
    {
        MailSender mailSender = new() { Id = Guid.Empty };
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSender(false))
            .Returns(value: new[] { mailSender }.AsQueryable());

        IActionResult result = controller.Get(key: mailSender.Id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedMailSender()
    {
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSender(false))
            .Returns(value: Array.Empty<MailSender>().AsQueryable());

        IActionResult result = controller.Get(key: Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newMailSender: new MailSender());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailSender: new MailSender());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchCannotFindMailSenderAsync()
    {
        mailSenderManagerMock.Setup(expression: service => service.GetMailSender(iMailSenderId: Guid.Empty))
            .Returns(value: null);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailSender: new Delta<MailSender>());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldUpdateMailSenderEventWhenPatchFindsCalendarAsync()
    {
        MailSender mailSender = new() { Id = Guid.Empty };
        mailSenderManagerMock.Setup(expression: service => service.GetMailSender(iMailSenderId: Guid.Empty))
            .Returns(value: mailSender);
        mailSenderManagerMock.Setup(expression: service => service.UpdateMailSenderAsync(mailSender))
            .ReturnsAsync(value: mailSender);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailSender: new Delta<MailSender>());

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        mailSenderManagerMock.Setup(expression: service => service.DeleteAsync(iMailSenderId: Guid.Empty))
            .ReturnsAsync(value: 1);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<NoContentResult>();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005