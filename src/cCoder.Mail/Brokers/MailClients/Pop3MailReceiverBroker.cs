using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class Pop3MailReceiverBroker : IPop3MailReceiverBroker
{
    public async Task<string[][]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateReceiveRequest(request);

        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(request.Host, request.Port, cancellationToken);

        using Stream networkStream = tcpClient.GetStream();
        using Stream stream = request.EnableSSL
            ? await CreateSslStreamAsync(networkStream, request.Host, cancellationToken)
            : networkStream;

        using StreamReader reader = new(stream, Encoding.ASCII, leaveOpen: true);
        using StreamWriter writer = new(stream, Encoding.ASCII, leaveOpen: true)
        {
            NewLine = "\r\n",
            AutoFlush = true,
        };

        await ExpectOkAsync(reader, cancellationToken);
        await SendCommandAsync(writer, reader, $"USER {request.User}", cancellationToken);
        await SendCommandAsync(writer, reader, $"PASS {request.Password}", cancellationToken);

        string stat = await SendCommandAsync(writer, reader, "STAT", cancellationToken);
        int count = ParseMessageCount(stat);
        int maximumMessages = request.MaximumMessages <= 0
            ? count
            : Math.Min(request.MaximumMessages, count);

        List<string[]> messages = [];

        for (int index = count; index > 0 && messages.Count < maximumMessages; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] rawMessage = await RetrieveMessageAsync(writer, reader, index, cancellationToken);
            messages.Add(rawMessage);
        }

        await writer.WriteLineAsync("QUIT");
        return [.. messages];
    }

    public Task<string[][]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        ReceiveAsync(
            new MailboxReceiveRequest
            {
                ProviderName = MailProviderNames.Pop3,
                Host = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_HOST"),
                Port = int.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_PORT"), out int port) ? port : 995,
                EnableSSL = !bool.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_SSL"), out bool enableSsl) || enableSsl,
                User = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_USER"),
                Password = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_PASSWORD"),
                MaximumMessages = count,
            },
            cancellationToken);

    private static async Task<Stream> CreateSslStreamAsync(
        Stream networkStream,
        string host,
        CancellationToken cancellationToken)
    {
        SslStream sslStream = new(networkStream, leaveInnerStreamOpen: false);
        await sslStream.AuthenticateAsClientAsync(host, null, enabledSslProtocols: default, checkCertificateRevocation: true);
        cancellationToken.ThrowIfCancellationRequested();
        return sslStream;
    }

    private static async Task<string> SendCommandAsync(
        StreamWriter writer,
        StreamReader reader,
        string command,
        CancellationToken cancellationToken)
    {
        await writer.WriteLineAsync(command);
        return await ExpectOkAsync(reader, cancellationToken);
    }

    private static async Task<string> ExpectOkAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        string line = await reader.ReadLineAsync(cancellationToken)
            ?? throw new InvalidOperationException("The mail server closed the connection.");

        if (!line.StartsWith("+OK", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(line);

        return line;
    }

    private static async Task<string[]> RetrieveMessageAsync(
        StreamWriter writer,
        StreamReader reader,
        int index,
        CancellationToken cancellationToken)
    {
        await SendCommandAsync(writer, reader, $"RETR {index}", cancellationToken);

        List<string> lines = [];

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (line == ".")
                break;

            lines.Add(line.StartsWith("..", StringComparison.Ordinal) ? line[1..] : line);
        }

        return [.. lines];
    }

    private static int ParseMessageCount(string stat)
    {
        string[] parts = stat.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 && int.TryParse(parts[1], out int count) ? count : 0;
    }

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

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

}
