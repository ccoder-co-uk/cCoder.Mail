// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.Linq;
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

        mailServerServiceMock.Setup(expression: x => x.GetAllMailServers())
            .Returns(value: entities);

        // When
        IQueryable<MailServer> result = mailServerProcessingService.GetAllMailServers();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        mailServerServiceMock.Verify(expression: x => x.GetAllMailServers(), times: Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005