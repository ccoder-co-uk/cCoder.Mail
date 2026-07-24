// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class ImapMailReceiverBroker : IImapMailReceiverBroker
{
    public async Task<MailClientTextConnection> OpenAsync(
        string host,
        int port,
        CancellationToken cancellationToken = default)
    {
        TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(host: host, port: port, cancellationToken: cancellationToken);
        Stream stream = tcpClient.GetStream();

        return CreateConnection(tcpClient: tcpClient, stream: stream);
    }

    public async Task<MailClientTextConnection> OpenSslAsync(
        string host,
        int port,
        CancellationToken cancellationToken = default)
    {
        TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(host: host, port: port, cancellationToken: cancellationToken);
        Stream networkStream = tcpClient.GetStream();
        SslStream sslStream = new(innerStream: networkStream, leaveInnerStreamOpen: false);
        await sslStream.AuthenticateAsClientAsync(targetHost: host, clientCertificates: null, enabledSslProtocols: default, checkCertificateRevocation: true);
        cancellationToken.ThrowIfCancellationRequested();

        return CreateConnection(tcpClient: tcpClient, stream: sslStream);
    }

    public Task WriteLineAsync(
        MailClientTextConnection connection,
        string line,
        CancellationToken cancellationToken = default) =>
        connection.Writer.WriteLineAsync(value: line);

    public Task<string> ReadLineAsync(
        MailClientTextConnection connection,
        CancellationToken cancellationToken = default) =>
        connection.Reader.ReadLineAsync(cancellationToken: cancellationToken)
        .AsTask();

    private static MailClientTextConnection CreateConnection(TcpClient tcpClient, Stream stream) =>
        new()
        {
            TcpClient = tcpClient,
            Stream = stream,
            Reader = new StreamReader(stream: stream, encoding: Encoding.ASCII, leaveOpen: true),
            Writer = new StreamWriter(stream: stream, encoding: Encoding.ASCII, leaveOpen: true)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            },
        };
}