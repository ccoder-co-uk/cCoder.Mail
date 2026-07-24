// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IImapMailReceiverBroker
{
    Task<MailClientTextConnection> OpenAsync(
        string host,
        int port,
        CancellationToken cancellationToken = default);

    Task<MailClientTextConnection> OpenSslAsync(
        string host,
        int port,
        CancellationToken cancellationToken = default);

    Task WriteLineAsync(
        MailClientTextConnection connection,
        string line,
        CancellationToken cancellationToken = default);

    Task<string> ReadLineAsync(
        MailClientTextConnection connection,
        CancellationToken cancellationToken = default);
}