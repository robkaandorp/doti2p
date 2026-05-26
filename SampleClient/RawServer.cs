using DotI2p;

using System.Text;

namespace SampleClient;

public class RawServer(SamSession samSession)
{
    public async Task StartAsync()
    {
        var rawSubSession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.DATAGRAM, 6969) /*{ FromPort = 6969, ListenPort = 6969 }*/);

        var replySubsession = await samSession.CreateDatagramSubSession(
            new DatagramSubSessionConfiguration(DatagramStyle.RAW, 0) /*{ FromPort = 6969, ListenPort = 6969 }*/);

        while (true)
        {
            var buffer = await rawSubSession.ReceiveAsync();
            var text = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(text);

            var newlineIdx = text.IndexOf('\n');
            var header = text.Substring(0, newlineIdx);

            var splittedHeader = header.Split(' ');
            var destination = splittedHeader.First();
            var port = ushort.Parse(splittedHeader.FirstOrDefault(s => s.StartsWith("FROM_PORT"))?.Substring("FROM_PORT".Length + 1) ?? "0");

            await replySubsession.SendAsync(destination, 6969, port, Encoding.UTF8.GetBytes("Reply: " + text.Substring(newlineIdx)));
        }
    }
}
