using DotI2p;

using System.Text;

namespace SampleClient;

public class RawServer(SamSession samSession)
{
    public async Task StartAsync()
    {
        var rawSubSession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.DATAGRAM, 6969) { FromPort = 6969, ListenPort = 6969 });

        while (true)
        {
            var buffer = await rawSubSession.ReceiveAsync();

            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }
    }
}
