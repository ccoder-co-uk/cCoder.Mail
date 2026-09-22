// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;
using cCoder.Mail.Providers.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class MailProviderServiceTests
{
    private readonly Mock<IMailClient> clientMock = new();
    private readonly Mock<IMailClientRegistryBroker> registryMock = new();
    private readonly Mock<IMailReceiverStorageBroker> storageMock = new();

    [Fact]
    public void GetMailClientShouldResolveAliasIgnoringCase()
    {
        registryMock.Setup(x => x.SelectMailClient("microsoftgraph"))
            .Returns(clientMock.Object);
        MailProviderOrchestrationService service = CreateService();

        IMailClient result = service.GetMailClient("microsoftgraph");

        result.Should().BeSameAs(clientMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("missing")]
    public void GetMailClientShouldRejectUnknownProvider(string providerName)
    {
        registryMock.Setup(x => x.SelectMailClient(providerName))
            .Throws(new InvalidOperationException(
                $"No mail provider named '{providerName}' is registered."));
        MailProviderOrchestrationService service = CreateService();

        Action action = () => service.GetMailClient(providerName);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"No mail provider named '{providerName}' is registered.");
    }

    [Fact]
    public async Task GetMailClientAsyncShouldResolveReceiverProviderAsync()
    {
        Guid id = Guid.NewGuid();
        clientMock.Setup(x => x.GetProviderNames()).Returns(["IMAP"]);
        storageMock.Setup(x => x.SelectMailReceiverByIdAsync(id, CancellationToken.None))
            .ReturnsAsync(new MailReceiver { Id = id, ProviderName = "IMAP" });
        registryMock.Setup(x => x.SelectMailClient("IMAP"))
            .Returns(clientMock.Object);
        MailProviderOrchestrationService service = CreateService();

        IMailClient result = await service.GetMailClientAsync(id);

        result.Should().BeSameAs(clientMock.Object);
    }

    [Fact]
    public async Task GetMailReceiverProviderNameAsyncShouldReturnProviderAsync()
    {
        Guid id = Guid.NewGuid();
        storageMock.Setup(x => x.SelectMailReceiverByIdAsync(id, CancellationToken.None))
            .ReturnsAsync(new MailReceiver { Id = id, ProviderName = "IMAP" });
        MailReceiverProviderService service = new(
            mailReceiverStorageBroker: storageMock.Object);

        string providerName = await service.RetrieveMailReceiverProviderNameAsync(id);

        providerName.Should().Be("IMAP");
    }

    [Fact]
    public async Task GetMailReceiverProviderNameAsyncShouldWrapMissingReceiverAsync()
    {
        Guid id = Guid.NewGuid();
        storageMock.Setup(x => x.SelectMailReceiverByIdAsync(id, CancellationToken.None))
            .ReturnsAsync((MailReceiver)null);
        MailReceiverProviderService service = new(
            mailReceiverStorageBroker: storageMock.Object);

        Func<Task> action = async () => await service.RetrieveMailReceiverProviderNameAsync(id);

        await action.Should().ThrowAsync<MailServiceException>();
    }

    [Fact]
    public async Task GetMailReceiverProviderNameAsyncShouldWrapMissingEmptyIdAsync()
    {
        MailReceiverProviderService service = new(
            mailReceiverStorageBroker: storageMock.Object);

        Func<Task> action = async () => await service.RetrieveMailReceiverProviderNameAsync(Guid.Empty);

        await action.Should().ThrowAsync<MailServiceException>();
    }

    private MailProviderOrchestrationService CreateService() =>
        new(
            mailProviderService: new MailProviderService(
                mailClientRegistryBroker: registryMock.Object),
            mailReceiverProviderService: new MailReceiverProviderService(
                mailReceiverStorageBroker: storageMock.Object));
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005