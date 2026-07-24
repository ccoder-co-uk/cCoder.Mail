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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<MailServer> entities = new[] { CreateRandomMailServer() }.AsQueryable();

        mailServerServiceMock.Setup(expression: x => x.GetAllMailServer())
            .Returns(value: entities);

        // When
        IQueryable<MailServer> result = mailServerProcessingService.GetAllMailServer();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        mailServerServiceMock.Verify(expression: x => x.GetAllMailServer(), times: Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}