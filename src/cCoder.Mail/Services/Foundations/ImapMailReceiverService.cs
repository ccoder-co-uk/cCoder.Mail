using System.Text;
using System.Text.RegularExpressions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class ImapMailReceiverService(IImapMailReceiverBroker imapMailReceiverBroker)
    : IImapMailReceiverService
{
    public async Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateReceiveRequest(request);

        using MailClientTextConnection connection = await OpenConnectionAsync(request, cancellationToken);
        _ = await imapMailReceiverBroker.ReadLineAsync(connection, cancellationToken);
        await SendCommandAsync(connection, "a1", $"LOGIN \"{Escape(request.User)}\" \"{Escape(request.Password)}\"", cancellationToken);
        await SendCommandAsync(connection, "a2", "SELECT INBOX", cancellationToken);

        string searchResponse = await SendCommandAsync(
            connection,
            "a3",
            BuildSearchCommand(request),
            cancellationToken);

        int[] messageIds = ParseSearchIds(searchResponse)
            .Reverse()
            .Take(Math.Clamp(request.MaximumMessages <= 0 ? 100 : request.MaximumMessages, 1, 100))
            .ToArray();

        List<ReceivedEmail> messages = [];

        foreach (int messageId in messageIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string rawMessage = await FetchMessageAsync(connection, messageId, cancellationToken);
            ReceivedEmail receivedEmail = ParseMessage(rawMessage);

            if (IsWithinPeriod(receivedEmail.ReceivedOn, request.From, request.To))
                messages.Add(receivedEmail);
        }

        await SendCommandAsync(connection, "az", "LOGOUT", cancellationToken);
        return [.. messages.OrderByDescending(message => message.ReceivedOn)];
    }

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        ReceiveAsync(
            new MailboxReceiveRequest
            {
                ProviderName = MailProviderNames.Imap,
                Host = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_HOST"),
                Port = int.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_PORT"), out int port) ? port : 993,
                EnableSSL = !bool.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_SSL"), out bool enableSsl) || enableSsl,
                User = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_USER"),
                Password = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_PASSWORD"),
                MaximumMessages = count,
            },
            cancellationToken);

    private Task<MailClientTextConnection> OpenConnectionAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken) =>
        request.EnableSSL
            ? imapMailReceiverBroker.OpenSslAsync(request.Host, request.Port, cancellationToken)
            : imapMailReceiverBroker.OpenAsync(request.Host, request.Port, cancellationToken);

    private async Task<string> SendCommandAsync(
        MailClientTextConnection connection,
        string tag,
        string command,
        CancellationToken cancellationToken)
    {
        await imapMailReceiverBroker.WriteLineAsync(connection, $"{tag} {command}", cancellationToken);

        StringBuilder response = new();

        while (await imapMailReceiverBroker.ReadLineAsync(connection, cancellationToken) is { } line)
        {
            _ = response.AppendLine(line);

            if (line.StartsWith($"{tag} OK", StringComparison.OrdinalIgnoreCase))
                return response.ToString();

            if (line.StartsWith($"{tag} NO", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith($"{tag} BAD", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(line);
            }
        }

        throw new InvalidOperationException("The mail server closed the IMAP connection.");
    }

    private async Task<string> FetchMessageAsync(
        MailClientTextConnection connection,
        int messageId,
        CancellationToken cancellationToken) =>
        await SendCommandAsync(connection, $"f{messageId}", $"FETCH {messageId} BODY[]", cancellationToken);

    private static string BuildSearchCommand(MailboxReceiveRequest request)
    {
        if (request.From is null)
            return "SEARCH ALL";

        return $"SEARCH SINCE {request.From.Value.UtcDateTime:dd-MMM-yyyy}";
    }

    private static int[] ParseSearchIds(string response) =>
        SearchRegex().Match(response).Groups["ids"].Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => int.TryParse(value, out int id) ? id : 0)
            .Where(id => id > 0)
            .ToArray();

    private static ReceivedEmail ParseMessage(string rawMessage)
    {
        string[] lines = rawMessage.Split('\n').Select(line => line.TrimEnd('\r')).ToArray();
        int headerStart = Array.FindIndex(lines, line => line.StartsWith("From:", StringComparison.OrdinalIgnoreCase));

        if (headerStart > 0)
            lines = lines[headerStart..];

        int separatorIndex = Array.FindIndex(lines, string.IsNullOrWhiteSpace);
        string[] headerLines = separatorIndex >= 0 ? lines[..separatorIndex] : lines;
        string[] bodyLines = separatorIndex >= 0 ? lines[(separatorIndex + 1)..] : [];
        Dictionary<string, string> headers = ParseHeaders(headerLines);

        return new()
        {
            MessageId = Header(headers, "Message-ID"),
            From = Header(headers, "From"),
            To = Header(headers, "To"),
            CC = Header(headers, "Cc"),
            Subject = DecodeHeader(Header(headers, "Subject")),
            Content = string.Join("\n", bodyLines).TrimEnd(')', '\r', '\n'),
            IsBodyHtml = Header(headers, "Content-Type")?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true,
            ReceivedOn = ParseDate(Header(headers, "Date")),
        };
    }

    private static Dictionary<string, string> ParseHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);
        string currentName = null;

        foreach (string line in lines)
        {
            if ((line.StartsWith(' ') || line.StartsWith('\t')) && currentName != null)
            {
                headers[currentName] += " " + line.Trim();
                continue;
            }

            int separatorIndex = line.IndexOf(':');

            if (separatorIndex <= 0)
                continue;

            currentName = line[..separatorIndex];
            headers[currentName] = line[(separatorIndex + 1)..].Trim();
        }

        return headers;
    }

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(name, out string value) ? value : null;

    private static string DecodeHeader(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? value
            : EncodedWordRegex().Replace(value, match =>
            {
                string encoding = match.Groups["encoding"].Value;
                string encodedText = match.Groups["text"].Value;

                return string.Equals(encoding, "B", StringComparison.OrdinalIgnoreCase)
                    ? Encoding.UTF8.GetString(Convert.FromBase64String(encodedText))
                    : encodedText.Replace('_', ' ');
            });

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(value, out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.MinValue;

    private static bool IsWithinPeriod(DateTimeOffset receivedOn, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (receivedOn == DateTimeOffset.MinValue)
            return from is null && to is null;

        return (from is null || receivedOn >= from.Value)
            && (to is null || receivedOn <= to.Value);
    }

    private static string Escape(string value) =>
        (value ?? string.Empty).Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Host))
            throw new InvalidOperationException("Mailbox host is required.");

        if (request.Port <= 0)
            throw new InvalidOperationException("Mailbox port is required.");

        if (string.IsNullOrWhiteSpace(request.User))
            throw new InvalidOperationException("Mailbox user is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Mailbox password is required.");
    }

    private static string ReadRequiredEnvironment(string variableName) =>
        ReadEnvironment(variableName)
        ?? throw new InvalidOperationException($"{variableName} is required to receive mailbox messages.");

    private static string ReadEnvironment(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [GeneratedRegex(@"^\* SEARCH (?<ids>.*)$", RegexOptions.Multiline)]
    private static partial Regex SearchRegex();

    [GeneratedRegex(@"=\?(?<charset>[^?]+)\?(?<encoding>[BQ])\?(?<text>[^?]+)\?=", RegexOptions.IgnoreCase)]
    private static partial Regex EncodedWordRegex();
}
