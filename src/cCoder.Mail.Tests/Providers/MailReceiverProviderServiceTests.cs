// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class MailReceiverProviderServiceTests
{
    private readonly Mock<IMailReceiverStorageBroker> storageMock = new();

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
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005