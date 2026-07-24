// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Sockets;

namespace cCoder.Mail.Dependencies.MailClients;

public sealed class MailClientTextConnection : IDisposable
{
    public TcpClient TcpClient { get; init; }

    public Stream Stream { get; init; }

    public StreamReader Reader { get; init; }

    public StreamWriter Writer { get; init; }

    public void Dispose()
    {
        Writer?.Dispose();
        Reader?.Dispose();
        Stream?.Dispose();
        TcpClient?.Dispose();
    }
}