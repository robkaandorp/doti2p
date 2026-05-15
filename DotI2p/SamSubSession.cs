namespace DotI2p
{
    public class SamSubSession
    {
        private readonly SamConnection connection;
        private readonly string subSessionId;

        public SamSubSession(SamConnection connection, string subSessionId)
        {
            this.connection = connection;
            this.subSessionId = subSessionId;
        }

        public SamVirtualStream CreateVirtualStream()
        {
            return new SamVirtualStream(connection.CreateClone(), subSessionId);
        }
    }
}
