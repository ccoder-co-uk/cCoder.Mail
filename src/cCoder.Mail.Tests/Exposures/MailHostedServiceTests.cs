// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005

using cCoder.Mail.Exposures.HostedServices;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class MailSenderHostedServiceTests
{
    [Fact]
    public async Task StartAsyncShouldRunSenderOrchestrationAsync()
    {
        TaskCompletionSource invoked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken actualCancellationToken = default;
        var hostedService = new MailSenderHostedService(
            runAsync: cancellationToken =>
            {
                actualCancellationToken = cancellationToken;
                invoked.SetResult();

                return Task.CompletedTask;
            });

        await hostedService.StartAsync(CancellationToken.None);
        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.True(actualCancellationToken.CanBeCanceled);
    }
}

public sealed partial class MailReceiverHostedServiceTests
{
    [Fact]
    public async Task StartAsyncShouldRunReceiverOrchestrationAsync()
    {
        TaskCompletionSource invoked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken actualCancellationToken = default;
        var hostedService = new MailReceiverHostedService(
            runAsync: cancellationToken =>
            {
                actualCancellationToken = cancellationToken;
                invoked.SetResult();

                return Task.CompletedTask;
            });

        await hostedService.StartAsync(CancellationToken.None);
        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.True(actualCancellationToken.CanBeCanceled);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXSTRUCT002, STXTEST005