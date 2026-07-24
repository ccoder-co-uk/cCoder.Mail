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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();

        sentEmailServiceMock.Setup(expression: x => x.UpdateSentEmailAsync(updatedSentEmail: entity))
            .ReturnsAsync(value: entity);

        // When
        SentEmail result = await sentEmailProcessingService.UpdateSentEmailAsync(updatedSentEmail: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        sentEmailServiceMock.Verify(expression: x => x.UpdateSentEmailAsync(updatedSentEmail: entity), times: Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}