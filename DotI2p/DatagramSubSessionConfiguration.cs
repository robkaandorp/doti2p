namespace DotI2p
{
    public enum DatagramStyle
    {
        DATAGRAM,
        RAW,
        DATAGRAM2,
        DATAGRAM3,
    }

    public class DatagramSubSessionConfiguration
    {
        public DatagramSubSessionConfiguration(DatagramStyle datagramStyle, ushort port)
        {
            this.Style = datagramStyle;
            this.Port = port;
        }

        public DatagramStyle Style { get; private set; }

        public ushort Port { get; private set; }

        public string? Host { get; set; }

        public ushort? FromPort { get; set; }

        public ushort? ToPort { get; set; }

        public byte? Protocol { get; set; }

        public ushort? ListenPort { get; set; }

        public ushort? ListenProtocol { get; set; }
    }
}
