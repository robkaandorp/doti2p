using DotI2p;

using System.Text;

namespace SampleClient;

public class RawServer(SamSession samSession)
{
    public async Task StartAsync()
    {
        var rawSubSession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.DATAGRAM, 6969));

        var replySubsession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.RAW, 0));

        while (true)
        {
            var result = await rawSubSession.ReceiveAsync();
            
            Console.WriteLine($"== From: {result.Destination?.GetB32Hostname()}:{result.FromPort} To: {result.ToPort} ==");
            var text = Encoding.UTF8.GetString(result.Data.Span);
            Console.WriteLine(text);

            if (result.Destination == null)
            {
                continue;
            }    

            await replySubsession.SendAsync(result.Destination.Destination, 6969, result.FromPort ?? 0, Encoding.UTF8.GetBytes("Reply: " + text));
        }
    }
}
