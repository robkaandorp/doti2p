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
        var subSession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.DATAGRAM, 0) { FromPort = 6970 });

        await subSession.SendAsync(remoteDestination.Destination, 6969, Encoding.UTF8.GetBytes("Hello, I2P!"));

        var responseSubsession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.RAW, 6970));
        var result = await responseSubsession.ReceiveAsync();

        Console.WriteLine(Encoding.UTF8.GetString(result.Data.Span));
    }
}
