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
        private readonly UdpClient udpClient;

        public SamDatagramSubSession(string subSessionId, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            this.subSessionId = subSessionId;
            this.remoteEndPoint = remoteEndPoint;
            this.udpClient = new UdpClient(localEndPoint);
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

        public async Task<byte[]> ReceiveAsync()
        {
            var result = await this.udpClient.ReceiveAsync();

            return result.Buffer;
        }
    }
}
