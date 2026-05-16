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
        private readonly int tcpPort;
        private readonly int udpPort;
        private TcpClient? tcpClient;
        private bool disposedValue;

        public IPAddress Host => this.host;

        public int TcpPort => this.tcpPort;

        public int UdpPort => this.udpPort;

        public SamConnection(int tcpPort = 7656, int udpPort = 7655)
            : this(IPAddress.Loopback, tcpPort, udpPort) { }

        public SamConnection(IPAddress host, int tcpPort = 7656, int udpPort = 7655)
        {
            this.host = host;
            this.tcpPort = tcpPort;
            this.udpPort = udpPort;
        }

        public async Task ConnectAsync()
        {
            this.tcpClient = new TcpClient();
            await this.tcpClient.ConnectAsync(this.host, this.tcpPort);

            var stream = this.tcpClient.GetStream();
            var response = await this.SendCommandAsync("HELLO VERSION MIN=3.3");

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
            if (this.tcpClient == null || !this.tcpClient.Connected)
            {
                throw new InvalidOperationException("Connection is not established.");
            }

            var stream = this.tcpClient.GetStream();

            await stream.WriteAsync(Encoding.UTF8.GetBytes(command + "\n"));
            await stream.FlushAsync();

            var response = await this.ReadLineAsync();

            return new CommandResponse(response);
        }

        public async Task<string> ReadLineAsync()
        {
            if (this.tcpClient == null || !this.tcpClient.Connected)
            {
                throw new InvalidOperationException("Connection is not established.");
            }

            var stream = this.tcpClient.GetStream();
            var buffer = new byte[4096];
            var i = 0;

            while (true)
            {
                var result = stream.ReadByte();

                if (result < 0)
                {
                    throw new IOException("Connection closed by remote host.");
                }

                if (result == '\r')
                {
                    continue;
                }

                if (result == '\n')
                {
                    break;
                }

                buffer[i++] = (byte)result;

                if (i >= buffer.Length)
                {
                    throw new IOException("Line too long.");
                }
            }

            return Encoding.UTF8.GetString(buffer, 0, i);
        }

        public SamConnection CreateClone()
        {
            return new SamConnection(this.host, this.tcpPort, this.udpPort);
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
                    this.tcpClient?.Close();
                    this.tcpClient = null;
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
