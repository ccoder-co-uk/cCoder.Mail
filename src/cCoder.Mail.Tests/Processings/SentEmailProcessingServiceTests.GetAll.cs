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

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<SentEmail> entities = new[] { CreateRandomSentEmail() }.AsQueryable();

        sentEmailServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<SentEmail> result = sentEmailProcessingService.GetAll();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        sentEmailServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}