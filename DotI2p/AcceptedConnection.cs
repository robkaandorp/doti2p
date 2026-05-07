using System.Net.Sockets;

namespace DotI2p
{
    public class AcceptedConnection
    {
        public AcceptedConnection(DestinationKey destination, TcpClient tcpClient)
        {
            Destination = destination;
            TcpClient = tcpClient;
        }

        public DestinationKey Destination { get; }

        public TcpClient TcpClient { get; }
    }
}
