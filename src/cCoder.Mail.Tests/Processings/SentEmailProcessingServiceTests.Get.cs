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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        var id = entity.Id;

        sentEmailServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        // When
        SentEmail result = sentEmailProcessingService.Get(id: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        sentEmailServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}