// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Text.RegularExpressions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class ImapMailReceiverService(
    IImapMailReceiverBroker imapMailReceiverBroker,
    MailConfiguration mailConfiguration)
    : IImapMailReceiverService
{
    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: async () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(inputs: [request, cancellationToken]);

            ValidateReceiveRequest(request: request);

            using MailClientTextConnection connection = await OpenConnectionAsync(request: request, cancellationToken: cancellationToken);
            _ = await imapMailReceiverBroker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken);
            await SendCommandAsync(connection: connection, tag: "a1", command: $"LOGIN \"{Escape(value: request.User)}\" \"{Escape(value: request.Password)}\"", cancellationToken: cancellationToken);
            await SendCommandAsync(connection: connection, tag: "a2", command: "SELECT INBOX", cancellationToken: cancellationToken);

            string searchResponse = await SendCommandAsync(
    connection: connection,
    tag: "a3",
    command: BuildSearchCommand(request: request),
    cancellationToken: cancellationToken);

            int[] messageIds = ParseSearchIds(response: searchResponse)
                .Reverse()
                .Take(count: Math.Clamp(value: request.MaximumMessages <= 0 ? 100 : request.MaximumMessages, min: 1, max: 100))
                .ToArray();

            List<ReceivedEmail> messages = [];

            foreach (int messageId in messageIds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string rawMessage = await FetchMessageAsync(connection: connection, messageId: messageId, cancellationToken: cancellationToken);
                ReceivedEmail receivedEmail = ParseMessage(rawMessage: rawMessage);

                if (IsWithinPeriod(receivedOn: receivedEmail.ReceivedOn, from: request.From, to: request.To))
                {
                    messages.Add(item: receivedEmail);
                }
            }

            await SendCommandAsync(connection: connection, tag: "az", command: "LOGOUT", cancellationToken: cancellationToken);
            return [.. messages.OrderByDescending(keySelector: message => message.ReceivedOn)];
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {

            ValidateReceiveTopAsync(inputs: [count, cancellationToken]);

            return ReceiveMailboxReceiveRequestAsync(
            request: new MailboxReceiveRequest
            {
                ProviderName = MailProviderNames.Imap,
                Host = ReadRequiredConfiguration(value: mailConfiguration.Imap.Host, configurationName: "IMAP mailbox host"),
                Port = mailConfiguration.Imap.Port,
                EnableSSL = mailConfiguration.Imap.EnableSSL,
                User = ReadRequiredConfiguration(value: mailConfiguration.Imap.User, configurationName: "IMAP mailbox user"),
                Password = ReadRequiredConfiguration(value: mailConfiguration.Imap.Password, configurationName: "IMAP mailbox password"),
                MaximumMessages = count,
            },
            cancellationToken: cancellationToken);
        }, isTask: true);

    private Task<MailClientTextConnection> OpenConnectionAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken) =>
        request.EnableSSL
            ? imapMailReceiverBroker.OpenSslAsync(host: request.Host, port: request.Port, cancellationToken: cancellationToken)
            : imapMailReceiverBroker.OpenAsync(host: request.Host, port: request.Port, cancellationToken: cancellationToken);

    private async Task<string> SendCommandAsync(
        MailClientTextConnection connection,
        string tag,
        string command,
        CancellationToken cancellationToken)
    {
        await imapMailReceiverBroker.WriteLineAsync(connection: connection, line: $"{tag} {command}", cancellationToken: cancellationToken);

        StringBuilder response = new();

        while (await imapMailReceiverBroker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken) is { } line)
        {
            _ = response.AppendLine(value: line);

            if (line.StartsWith(value: $"{tag} OK", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return response.ToString();
            }

            if (line.StartsWith(value: $"{tag} NO", comparisonType: StringComparison.OrdinalIgnoreCase)
                || line.StartsWith(value: $"{tag} BAD", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(message: line);
            }
        }

        throw new InvalidOperationException(message: "The mail server closed the IMAP connection.");
    }

    private Task<string> FetchMessageAsync(
        MailClientTextConnection connection,
        int messageId,
        CancellationToken cancellationToken) =>
        SendCommandAsync(
            connection: connection,
            tag: $"f{messageId}",
            command: $"FETCH {messageId} BODY[]",
            cancellationToken: cancellationToken);

    private static string BuildSearchCommand(MailboxReceiveRequest request)
    {
        if (request.From is null)
        {
            return "SEARCH ALL";
        }

        return $"SEARCH SINCE {request.From.Value.UtcDateTime:dd-MMM-yyyy}";
    }

    private static int[] ParseSearchIds(string response) =>
        SearchRegex()
        .Match(input: response)
        .Groups["ids"].Value
            .Split(separator: ' ', options: StringSplitOptions.RemoveEmptyEntries)
        .Select(selector: value => int.TryParse(s: value, result: out int id) ? id : 0)
        .Where(predicate: imapMailReceiverId => imapMailReceiverId > 0)
        .ToArray();

    private static ReceivedEmail ParseMessage(string rawMessage)
    {
        string[] lines = rawMessage.Split(separator: '\n')
            .Select(selector: line => line.TrimEnd(trimChar: '\r'))
            .ToArray();

        int headerStart = Array.FindIndex(array: lines, match: line => line.StartsWith(value: "From:", comparisonType: StringComparison.OrdinalIgnoreCase));

        if (headerStart > 0)
        {
            lines = lines[headerStart..];
        }

        int separatorIndex = Array.FindIndex(array: lines, match: string.IsNullOrWhiteSpace);
        string[] headerLines = separatorIndex >= 0 ? lines[..separatorIndex] : lines;
        string[] bodyLines = separatorIndex >= 0 ? lines[(separatorIndex + 1)..] : [];
        Dictionary<string, string> headers = ParseHeaders(lines: headerLines);

        return new()
        {
            MessageId = Header(headers: headers, name: "Message-ID"),
            From = Header(headers: headers, name: "From"),
            To = Header(headers: headers, name: "To"),
            CC = Header(headers: headers, name: "Cc"),
            Subject = DecodeHeader(value: Header(headers: headers, name: "Subject")),
            Content = string.Join(separator: "\n", value: bodyLines)
            .TrimEnd(trimChars: [')', '\r', '\n']),
            IsBodyHtml = Header(headers: headers, name: "Content-Type")?.StartsWith(value: "text/html", comparisonType: StringComparison.OrdinalIgnoreCase) == true,
            ReceivedOn = ParseDate(value: Header(headers: headers, name: "Date")),
        };
    }

    private static Dictionary<string, string> ParseHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new(comparer: StringComparer.OrdinalIgnoreCase);
        string currentName = null;

        foreach (string line in lines)
        {
            if ((line.StartsWith(value: ' ') || line.StartsWith(value: '\t')) && currentName != null)
            {
                headers[currentName] += " " + line.Trim();
                continue;
            }

            int separatorIndex = line.IndexOf(value: ':');

            if (separatorIndex <= 0)
            {
                continue;
            }

            currentName = line[..separatorIndex];
            headers[currentName] = line[(separatorIndex + 1)..].Trim();
        }

        return headers;
    }

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(key: name, value: out string value) ? value : null;

    private static string DecodeHeader(string value) =>
        string.IsNullOrWhiteSpace(value: value)
            ? value
            : EncodedWordRegex()
        .Replace(input: value, evaluator: match =>
            {
                string encoding = match.Groups["encoding"].Value;
                string encodedText = match.Groups["text"].Value;

                return string.Equals(a: encoding, b: "B", comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? Encoding.UTF8.GetString(bytes: Convert.FromBase64String(s: encodedText))
                    : encodedText.Replace(oldChar: '_', newChar: ' ');
            });

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(input: value, result: out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.MinValue;

    private static bool IsWithinPeriod(DateTimeOffset receivedOn, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (receivedOn == DateTimeOffset.MinValue)
        {
            return from is null && to is null;
        }

        return (from is null || receivedOn >= from.Value)
            && (to is null || receivedOn <= to.Value);
    }

    private static string Escape(string value) =>
        (value ?? string.Empty).Replace(oldValue: "\\", newValue: "\\\\", comparisonType: StringComparison.Ordinal)
        .Replace(oldValue: "\"", newValue: "\\\"", comparisonType: StringComparison.Ordinal);

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(paramName: nameof(request));
        }

        if (string.IsNullOrWhiteSpace(value: request.Host))
        {
            throw new InvalidOperationException(message: "Mailbox host is required.");
        }

        if (request.Port <= 0)
        {
            throw new InvalidOperationException(message: "Mailbox port is required.");
        }

        if (string.IsNullOrWhiteSpace(value: request.User))
        {
            throw new InvalidOperationException(message: "Mailbox user is required.");
        }

        if (string.IsNullOrWhiteSpace(value: request.Password))
        {
            throw new InvalidOperationException(message: "Mailbox password is required.");
        }
    }

    private static string ReadRequiredConfiguration(string value, string configurationName) =>
        string.IsNullOrWhiteSpace(value: value)
            ? throw new InvalidOperationException(message: $"{configurationName} is required to receive mailbox messages.")
            : value;

    [GeneratedRegex(@"^\* SEARCH (?<ids>.*)$", RegexOptions.Multiline)]
    private static partial Regex SearchRegex();

    [GeneratedRegex(@"=\?(?<charset>[^?]+)\?(?<encoding>[BQ])\?(?<text>[^?]+)\?=", RegexOptions.IgnoreCase)]
    private static partial Regex EncodedWordRegex();
}