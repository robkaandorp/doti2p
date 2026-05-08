using DotI2p;

var samConnection = new SamConnection();
await samConnection.ConnectAsync();

var samSession = new SamSession(samConnection);
var destination = await samSession.CreateStreamAsync();

Console.WriteLine($"SAM session created. Destination: {destination.GetB32Hostname()}");
Console.WriteLine();

if (args.Length > 0)
{
    DestinationKey? remoteDestination = null;

    if (args[0].EndsWith(".b32.i2p"))
    {
        remoteDestination = await samSession.HostNameLookupAsync(args[0]);
    }
    else
    {
        remoteDestination = new DestinationKey(args[0]);
    }

    var virtualStream = samSession.CreateVirtualStream();
    var client = await virtualStream.ConnectAsync(remoteDestination);

    Console.WriteLine($"Connected to {args[0]}");

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
else
{
    List<StreamWriter> writers = [];

    _ = Task.Run(async () =>
    {
        while (true)
        {
            using var virtualStream = samSession.CreateVirtualStream();
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
