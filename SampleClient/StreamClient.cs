using DotI2p;

namespace SampleClient;

public class StreamClient
{
    private readonly SamSession samSession;
    private readonly DestinationKey remoteDestination;

    public StreamClient(SamSession samSession, DestinationKey remoteDestination)
    {
        this.samSession = samSession;
        this.remoteDestination = remoteDestination;
    }

    public async Task StartAsync()
    {
        var samSubSession = await samSession.CreateStreamSubSession();
        var virtualStream = samSubSession.CreateVirtualStream();
        var client = await virtualStream.ConnectAsync(remoteDestination);

        Console.WriteLine($"Connected to {remoteDestination.GetB32Hostname()}");

        var clientStream = client.GetStream();
        var writer = new StreamWriter(clientStream);
        var reader = new StreamReader(clientStream);

        _ = Task.Run(async () =>
        {
            while (await reader.ReadLineAsync() is string line)
            {
                Console.WriteLine($" > {line}");
            }
        });

        while (Console.ReadLine() is string line)
        {
            writer.WriteLine(line);
            writer.Flush();
        }
    }
}
