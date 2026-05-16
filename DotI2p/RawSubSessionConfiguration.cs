namespace DotI2p
{
    public class RawSubSessionConfiguration
    {
        public ushort Port { get; set; }

        public string? Host { get; set; }

        public ushort? FromPort { get; set; }

        public ushort? ToPort { get; set; }

        public ushort? Protocol { get; set; }

        public ushort? ListenPort { get; set; }

        public ushort? ListenProtocol { get; set; }
    }
}
