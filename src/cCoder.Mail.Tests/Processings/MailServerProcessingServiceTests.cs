// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    private DataUser currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IMailServerService> mailServerServiceMock = new();
    private readonly MailServerProcessingService mailServerProcessingService;

    public MailServerProcessingServiceTests()
    {
        mailServerProcessingService = new MailServerProcessingService(service: mailServerServiceMock.Object);
    }

    private static MailServer CreateRandomMailServer() =>
        Builder<MailServer>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.AppId = 1)
        .With(func: x => x.Name = $"MailServer-{Guid.NewGuid():N}")
        .With(func: x => x.App = null)
        .Build();
}