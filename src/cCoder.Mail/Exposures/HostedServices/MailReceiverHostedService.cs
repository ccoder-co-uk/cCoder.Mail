// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Hosting;

namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailReceiverHostedService(
    Func<CancellationToken, Task> runAsync)
    : BackgroundService,
        IMailReceiverHostedService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        runAsync.Invoke(arg: stoppingToken);
}