// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;

namespace cCoder.Mail.Services.Foundations;

public sealed partial class MailMetadataTypeServiceTests
{
    [Fact]
    public void GetKnownMetadataShouldDescribeMailEntities()
    {
        var service = new MailMetadataTypeService();

        var metadata = service.GetKnownMetadata().Single();

        metadata.Name.Should().Be("Mail");
        metadata.UriBase.Should().Be("Mail");
        metadata.Types.Should().HaveCount(3);
        metadata.Types.Should().OnlyContain(type => type.IsEntity && type.HasEndpoint);
        metadata.Types.Should().OnlyContain(type => type.Category == "Mail");
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005