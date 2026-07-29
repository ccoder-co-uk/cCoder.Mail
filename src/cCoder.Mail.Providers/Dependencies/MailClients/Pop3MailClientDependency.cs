// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Dependencies.MailClients;

internal sealed class Pop3MailClientDependency : TcpClient
{
    internal async Task<string[][]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        await ConnectAsync(
            host: request.Host,
            port: request.Port,
            cancellationToken: cancellationToken);

        await using Stream stream = await CreateStreamAsync(
            host: request.Host,
            enableSsl: request.EnableSSL,
            cancellationToken: cancellationToken);

        using StreamReader reader = new(
            stream: stream,
            encoding: Encoding.ASCII,
            leaveOpen: true);

        await using StreamWriter writer = new(
            stream: stream,
            encoding: Encoding.ASCII,
            leaveOpen: true)
        {
            NewLine = "\r\n",
            AutoFlush = true,
        };

        await ExpectOkAsync(
            reader: reader,
            cancellationToken: cancellationToken);

        await SendCommandAsync(
            reader: reader,
            writer: writer,
            command: $"USER {request.User}",
            cancellationToken: cancellationToken);

        await SendCommandAsync(
            reader: reader,
            writer: writer,
            command: $"PASS {request.Password}",
            cancellationToken: cancellationToken);

        string stat = await SendCommandAsync(
            reader: reader,
            writer: writer,
            command: "STAT",
            cancellationToken: cancellationToken);

        int count = ParseMessageCount(stat: stat);
        int maximumMessages = request.MaximumMessages <= 0
            ? count
            : Math.Min(
                val1: request.MaximumMessages,
                val2: count);

        List<string[]> messages = [];

        for (int index = count;
            index > 0 && messages.Count < maximumMessages;
            index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            messages.Add(
                item: await RetrieveMessageAsync(
                    reader: reader,
                    writer: writer,
                    index: index,
                    cancellationToken: cancellationToken));
        }

        await writer.WriteLineAsync(value: "QUIT");

        return [.. messages];
    }

    private async Task<Stream> CreateStreamAsync(
        string host,
        bool enableSsl,
        CancellationToken cancellationToken)
    {
        Stream stream = GetStream();

        if (!enableSsl)
        {
            return stream;
        }

        SslStream sslStream = new(
            innerStream: stream,
            leaveInnerStreamOpen: false);

        await sslStream.AuthenticateAsClientAsync(
            targetHost: host,
            clientCertificates: null,
            enabledSslProtocols: default,
            checkCertificateRevocation: true);

        cancellationToken.ThrowIfCancellationRequested();

        return sslStream;
    }

    private static async Task<string> SendCommandAsync(
        StreamReader reader,
        StreamWriter writer,
        string command,
        CancellationToken cancellationToken)
    {
        await writer.WriteLineAsync(value: command);

        return await ExpectOkAsync(
            reader: reader,
            cancellationToken: cancellationToken);
    }

    private static async Task<string> ExpectOkAsync(
        StreamReader reader,
        CancellationToken cancellationToken)
    {
        string line = await reader.ReadLineAsync(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                message: "The mail server closed the connection.");

        if (!line.StartsWith(
            value: "+OK",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(message: line);
        }

        return line;
    }

    private static async Task<string[]> RetrieveMessageAsync(
        StreamReader reader,
        StreamWriter writer,
        int index,
        CancellationToken cancellationToken)
    {
        await SendCommandAsync(
            reader: reader,
            writer: writer,
            command: $"RETR {index}",
            cancellationToken: cancellationToken);

        List<string> lines = [];

        while (await reader.ReadLineAsync(
            cancellationToken: cancellationToken) is { } line)
        {
            if (line == ".")
            {
                break;
            }

            lines.Add(
                item: line.StartsWith(
                    value: "..",
                    comparisonType: StringComparison.Ordinal)
                    ? line[1..]
                    : line);
        }

        return [.. lines];
    }

    private static int ParseMessageCount(string stat)
    {
        string[] parts = stat.Split(
            separator: ' ',
            options: StringSplitOptions.RemoveEmptyEntries);

        return parts.Length >= 2
            && int.TryParse(
                s: parts[1],
                result: out int count)
            ? count
            : 0;
    }
}