// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Dependencies.MailClients;

internal sealed class ImapMailClientDependency : TcpClient
{
    internal async Task<string[]> ReceiveAsync(
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

        _ = await reader.ReadLineAsync(
            cancellationToken: cancellationToken);

        await SendCommandAsync(
            reader: reader,
            writer: writer,
            tag: "a1",
            command:
                $"LOGIN \"{Escape(value: request.User)}\" \"{Escape(value: request.Password)}\"",
            cancellationToken: cancellationToken);

        await SendCommandAsync(
            reader: reader,
            writer: writer,
            tag: "a2",
            command: "SELECT INBOX",
            cancellationToken: cancellationToken);

        string searchResponse = await SendCommandAsync(
            reader: reader,
            writer: writer,
            tag: "a3",
            command: BuildSearchCommand(request: request),
            cancellationToken: cancellationToken);

        int maximumMessages = request.MaximumMessages <= 0
            ? 100
            : request.MaximumMessages;

        int[] messageIds = ParseSearchIds(response: searchResponse)
            .Reverse()
            .Take(
                count: Math.Clamp(
                    value: maximumMessages,
                    min: 1,
                    max: 100))
            .ToArray();

        List<string> messages = [];

        foreach (int messageId in messageIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            messages.Add(
                item: await SendCommandAsync(
                    reader: reader,
                    writer: writer,
                    tag: $"f{messageId}",
                    command: $"FETCH {messageId} BODY[]",
                    cancellationToken: cancellationToken));
        }

        await SendCommandAsync(
            reader: reader,
            writer: writer,
            tag: "az",
            command: "LOGOUT",
            cancellationToken: cancellationToken);

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
        string tag,
        string command,
        CancellationToken cancellationToken)
    {
        await writer.WriteLineAsync(
            value: $"{tag} {command}");

        StringBuilder response = new();

        while (await reader.ReadLineAsync(
            cancellationToken: cancellationToken) is { } line)
        {
            _ = response.AppendLine(value: line);

            if (line.StartsWith(
                value: $"{tag} OK",
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return response.ToString();
            }

            if (line.StartsWith(
                value: $"{tag} NO",
                comparisonType: StringComparison.OrdinalIgnoreCase)
                || line.StartsWith(
                    value: $"{tag} BAD",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(message: line);
            }
        }

        throw new InvalidOperationException(
            message: "The mail server closed the IMAP connection.");
    }

    private static string BuildSearchCommand(
        MailboxReceiveRequest request) =>
        request.From is null
            ? "SEARCH ALL"
            : $"SEARCH SINCE {request.From.Value.UtcDateTime:dd-MMM-yyyy}";

    private static int[] ParseSearchIds(string response)
    {
        const string searchPrefix = "* SEARCH ";

        string searchLine = response
            .Split(separator: '\n')
            .FirstOrDefault(
                predicate: line =>
                    line.StartsWith(
                        value: searchPrefix,
                        comparisonType: StringComparison.Ordinal))
            ?? string.Empty;

        return searchLine[searchPrefix.Length..]
            .Split(
                separator: ' ',
                options: StringSplitOptions.RemoveEmptyEntries)
            .Select(
                selector: value =>
                    int.TryParse(
                        s: value,
                        result: out int id)
                        ? id
                        : 0)
            .Where(predicate: id => id > 0)
            .ToArray();
    }

    private static string Escape(string value) =>
        (value ?? string.Empty)
            .Replace(
                oldValue: "\\",
                newValue: "\\\\",
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: "\"",
                newValue: "\\\"",
                comparisonType: StringComparison.Ordinal);
}