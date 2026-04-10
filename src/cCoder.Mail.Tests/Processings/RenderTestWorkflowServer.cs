using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cCoder.Mail.Tests.Processings;

internal static class RenderTestWorkflowServer
{
    internal static async Task<T> RunAsync<T>(Func<string, T> action, string responseBody = "executed")
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        Task serverTask = Task.Run(async () =>
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream, Encoding.UTF8, false, 1024, true);

            int contentLength = 0;
            while (true)
            {
                string line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                    break;

                if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                    int.TryParse(line["Content-Length:".Length..].Trim(), out contentLength);
            }

            if (contentLength > 0)
            {
                char[] bodyBuffer = new char[contentLength];
                _ = await reader.ReadBlockAsync(bodyBuffer, 0, contentLength);
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseBody);
            string header =
                $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {responseBytes.Length}\r\nConnection: close\r\n\r\n";
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);

            await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            await stream.FlushAsync();
        });

        T result = action($"http://127.0.0.1:{port}/");
        await serverTask;
        return result;
    }
}

