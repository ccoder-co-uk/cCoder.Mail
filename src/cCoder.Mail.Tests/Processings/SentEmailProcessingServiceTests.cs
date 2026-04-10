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

public partial class SentEmailProcessingServiceTests
{
    private DataUser currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ISentEmailService> sentEmailServiceMock = new();
    private readonly SentEmailProcessingService sentEmailProcessingService;

    public SentEmailProcessingServiceTests()
    {
        sentEmailProcessingService = new SentEmailProcessingService(sentEmailServiceMock.Object);
    }

    private static SentEmail CreateRandomSentEmail() =>
        Builder<SentEmail>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.AppId = 1)
            .With(x => x.SentByUserId = "test-user")
            .With(x => x.Subject = $"Subject-{Guid.NewGuid():N}")
            .With(x => x.Content = $"Content-{Guid.NewGuid():N}")
            .With(x => x.To = $"{Guid.NewGuid():N}@example.com")
            .With(x => x.CC = string.Empty)
            .With(x => x.App = null)
            .With(x => x.SentBy = null)
            .With(x => x.From = "from@example.com")
            .Build();
}










