using DotI2p;

using System.Text;

namespace SampleClient;

public class RawServer(SamSession samSession)
{
    public async Task StartAsync()
    {
        var rawSubSession = await samSession.CreateRawSubSession(
            new RawSubSessionConfiguration { Port = 6969 });

        while (true)
        {
            var buffer = await rawSubSession.ReceiveAsync();

            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }
    }
}
