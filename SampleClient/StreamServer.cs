using DotI2p;

namespace SampleClient;

public class StreamServer
{
    private SamSession samSession;

    public StreamServer(SamSession samSession)
    {
        this.samSession = samSession;
    }

    public async Task StartAsync()
    {
        List<StreamWriter> writers = [];

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var samSubSession = await samSession.CreateStreamSubSession();
                var virtualStream = samSubSession.CreateVirtualStream();
                var acceptedConnection = await virtualStream.AcceptAsync();

                Console.WriteLine($"Accepted connection from {acceptedConnection.Destination.GetB32Hostname()}");

                var clientStream = acceptedConnection.TcpClient.GetStream();

                lock (writers)
                {
                    writers.Add(new StreamWriter(clientStream));
                }

                _ = Task.Run(async () =>
                {
                    var reader = new StreamReader(clientStream);

                    while (await reader.ReadLineAsync() is string line)
                    {
                        Console.WriteLine($"{acceptedConnection.Destination.GetB32Hostname()[..8]}> {line}");
                    }
                });
            }
        });

        while (Console.ReadLine() is string line)
        {
            lock (writers)
            {
                foreach (var writer in writers)
                {
                    writer.WriteLine(line);
                    writer.Flush();
                }
            }
        }
    }
}
