namespace DotI2p
{
    public class SamStreamSubSession
    {
        private readonly SamConnection connection;
        private readonly string subSessionId;

        public SamStreamSubSession(SamConnection connection, string subSessionId)
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
