// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        var id = entity.Id;

        mailServerServiceMock.Setup(expression: x => x.DeleteAsync(iMailServerId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await mailServerProcessingService.DeleteAllMailServerAsync(deletedMailServer: new[] { entity });

        // Then
        mailServerServiceMock.Verify(expression: x => x.DeleteAsync(iMailServerId: id), times: Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}