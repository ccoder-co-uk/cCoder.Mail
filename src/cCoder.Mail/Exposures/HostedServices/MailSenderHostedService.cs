// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailSenderHostedService(
    Func<CancellationToken, Task> runAsync)
    : BackgroundService,
        IMailSenderHostedService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        runAsync.Invoke(arg: stoppingToken);
}