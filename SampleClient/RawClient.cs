using DotI2p;

using System.Text;

namespace SampleClient;

public class RawClient
{
    private SamSession samSession;
    private DestinationKey remoteDestination;

    public RawClient(SamSession samSession, DestinationKey remoteDestination)
    {
        this.samSession = samSession;
        this.remoteDestination = remoteDestination;
    }

    public async Task StartAsync()
    {
        var subSession = await samSession.CreateRawSubSession(
            new RawSubSessionConfiguration { Port = 6970 });

        await subSession.SendAsync(remoteDestination.Destination, null, null, null, Encoding.UTF8.GetBytes("Hello, I2P!"));
    }
}
