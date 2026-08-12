// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005

using cCoder.Mail.Exposures.HostedServices;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class MailSenderHostedServiceTests
{
    [Fact]
    public async Task StartAsyncShouldRunSenderOrchestrationAsync()
    {
        Mock<IMailSenderOrchestrationService> orchestrationMock = new();
        TaskCompletionSource invoked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        orchestrationMock.Setup(x => x.RunContinuouslyAsync(It.IsAny<CancellationToken>()))
            .Callback(() => invoked.SetResult()).Returns(Task.CompletedTask);
        ServiceProvider provider = new ServiceCollection().AddScoped(_ => orchestrationMock.Object).BuildServiceProvider();
        var hostedService = new MailSenderHostedService(provider.GetRequiredService<IServiceScopeFactory>());

        await hostedService.StartAsync(CancellationToken.None);
        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(2));

        orchestrationMock.Verify(x => x.RunContinuouslyAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public sealed partial class MailReceiverHostedServiceTests
{
    [Fact]
    public async Task StartAsyncShouldRunReceiverOrchestrationAsync()
    {
        Mock<IMailReceiverOrchestrationService> orchestrationMock = new();
        TaskCompletionSource invoked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        orchestrationMock.Setup(x => x.RunContinuouslyAsync(It.IsAny<CancellationToken>()))
            .Callback(() => invoked.SetResult()).Returns(Task.CompletedTask);
        ServiceProvider provider = new ServiceCollection().AddScoped(_ => orchestrationMock.Object).BuildServiceProvider();
        var hostedService = new MailReceiverHostedService(provider.GetRequiredService<IServiceScopeFactory>());

        await hostedService.StartAsync(CancellationToken.None);
        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(2));

        orchestrationMock.Verify(x => x.RunContinuouslyAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005