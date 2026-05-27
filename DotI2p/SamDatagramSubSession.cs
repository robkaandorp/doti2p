using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotI2p
{
    public class SamDatagramSubSession
    {
        private readonly string subSessionId;
        private readonly IPEndPoint remoteEndPoint;
        private readonly DatagramStyle style;
        private readonly UdpClient udpClient;

        public SamDatagramSubSession(string subSessionId, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, DatagramStyle style)
        {
            this.subSessionId = subSessionId;
            this.remoteEndPoint = remoteEndPoint;
            this.style = style;
            this.udpClient = new UdpClient(localEndPoint);
        }

        public async Task SendAsync(string destination, ushort toPort, byte[] data)
        {
            await this.SendAsync(destination, null, toPort, null, data);
        }

        public async Task SendAsync(string destination, ushort fromPort, ushort toPort, byte[] data)
        {
            await this.SendAsync(destination, fromPort, toPort, null, data);
        }

        public async Task SendAsync(string destination, ushort? fromPort, ushort? toPort, ushort? protocol, byte[] data)
        {
            var command = $"3.3 {this.subSessionId} {destination}";

            if (fromPort != null)
            {
                command += $" FROM_PORT={fromPort}";
            }

            if (toPort != null)
            {
                command += $" TO_PORT={toPort}";
            }

            if (protocol != null)
            {
                command += $" PROTOCOL={protocol}";
            }

            var line = Encoding.UTF8.GetBytes(command + "\n");
            var buffer = new byte[line.Length + data.Length];
            line.CopyTo(buffer, 0);
            data.CopyTo(buffer, line.Length);

            await this.udpClient.SendAsync(buffer, buffer.Length, remoteEndPoint);
        }

        public async Task<DatagramReceiveResult> ReceiveAsync()
        {
            var result = await this.udpClient.ReceiveAsync();

            if (style == DatagramStyle.RAW)
            {
                return new DatagramReceiveResult(result.Buffer);
            }

            var newlineIdx = Array.IndexOf(result.Buffer, (byte)'\n');

            if (newlineIdx < 0)
            {
                return new DatagramReceiveResult(result.Buffer);
            }

            var header = Encoding.UTF8.GetString(result.Buffer, 0, newlineIdx);
            var splittedHeader = header.Split(' ');

            var destination = splittedHeader[0];
            var fromPort = splittedHeader.SingleOrDefault(s => s.StartsWith("FROM_PORT="))?.Substring(10);
            var toPort = splittedHeader.SingleOrDefault(s => s.StartsWith("TO_PORT="))?.Substring(8);

            ushort? fromPortValue = null;
            if (fromPort != null)
            {
                fromPortValue = ushort.Parse(fromPort);
            }

            ushort? toPortValue = null;
            if (toPort != null)
            {
                toPortValue = ushort.Parse(toPort);
            }

            return new DatagramReceiveResult(destination, fromPortValue, toPortValue, result.Buffer.AsMemory().Slice(newlineIdx + 1));
        }
    }

    public class DatagramReceiveResult
    {
        public string? Destination { get; }

        public ushort? FromPort { get; }

        public ushort? ToPort { get; }

        public ReadOnlyMemory<byte> Data { get; }

        public DatagramReceiveResult(ReadOnlyMemory<byte> data)
        {
            this.Data = data;
        }

        public DatagramReceiveResult(string destination, ushort? fromPort, ushort? toPort, ReadOnlyMemory<byte> data)
        {
            this.Destination = destination;
            this.FromPort = fromPort;
            this.ToPort = toPort;
            this.Data = data;
        }
    }
}
