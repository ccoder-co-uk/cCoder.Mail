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
        await tcpClient.ConnectAsync(host, port, cancellationToken);
        Stream stream = tcpClient.GetStream();

        return CreateConnection(tcpClient, stream);
    }

    public async Task<MailClientTextConnection> OpenSslAsync(
        string host,
        int port,
        CancellationToken cancellationToken = default)
    {
        TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(host, port, cancellationToken);
        Stream networkStream = tcpClient.GetStream();
        SslStream sslStream = new(networkStream, leaveInnerStreamOpen: false);
        await sslStream.AuthenticateAsClientAsync(host, null, enabledSslProtocols: default, checkCertificateRevocation: true);
        cancellationToken.ThrowIfCancellationRequested();

        return CreateConnection(tcpClient, sslStream);
    }

    public Task WriteLineAsync(
        MailClientTextConnection connection,
        string line,
        CancellationToken cancellationToken = default) =>
        connection.Writer.WriteLineAsync(line);

    public Task<string> ReadLineAsync(
        MailClientTextConnection connection,
        CancellationToken cancellationToken = default) =>
        connection.Reader.ReadLineAsync(cancellationToken).AsTask();

    private static MailClientTextConnection CreateConnection(TcpClient tcpClient, Stream stream) =>
        new()
        {
            TcpClient = tcpClient,
            Stream = stream,
            Reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true),
            Writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            },
        };
}
