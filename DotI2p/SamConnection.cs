using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotI2p
{
    public class SamConnection : IDisposable
    {
        private readonly IPAddress host;
        private readonly int port;
        private TcpClient? tcpClient;
        private StreamWriter? writer;
        private StreamReader? reader;
        private bool disposedValue;

        public SamConnection(int port = 7656)
            : this(IPAddress.Loopback, port) { }

        public SamConnection(IPAddress host, int port = 7656)
        {
            this.host = host;
            this.port = port;
        }

        public async Task ConnectAsync()
        {
            this.tcpClient = new TcpClient();
            await this.tcpClient.ConnectAsync(this.host, this.port);

            var stream = this.tcpClient.GetStream();
            this.writer = new StreamWriter(stream);
            this.reader = new StreamReader(stream);

            var response = await this.SendCommandAsync("HELLO VERSION MIN=3.1 MAX=3.1");

            if (!response.Response.Equals("HELLO REPLY", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }
        }

        public async Task<CommandResponse> SendCommandAsync(string command)
        {
            if (this.writer == null || this.reader == null)
            {
                throw new InvalidOperationException("Connection is not established.");
            }

            this.writer.Write(command + "\n");
            this.writer.Flush();

            var response = await this.reader.ReadLineAsync();

            return new CommandResponse(response);
        }

        public async Task<string?> ReadLineAsync()
        {
            if (this.writer == null || this.reader == null)
            {
                throw new InvalidOperationException("Connection is not established.");
            }
            
            return await this.reader.ReadLineAsync();
        }

        public SamConnection CreateClone()
        {
            return new SamConnection(this.host, this.port);
        }

        public TcpClient GetTcpClient()
        {
            if (this.tcpClient == null)
            {
                throw new InvalidOperationException("Connection is not established.");
            }

            return this.tcpClient;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.writer?.Dispose();
                    this.reader?.Dispose();
                    this.tcpClient?.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
