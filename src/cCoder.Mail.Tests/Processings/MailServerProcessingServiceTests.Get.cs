// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        var id = entity.Id;

        mailServerServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        // When
        MailServer result = mailServerProcessingService.Get(id: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        mailServerServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}