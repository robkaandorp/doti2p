using DotI2p;

using SampleClient;

using System.Text.Json;

DestinationKey? destination = null;

if (args.Length == 0)
{
    // Server mode: reuse the previously saved destination.
    if (File.Exists("destination.json"))
    {
        var destinationJson = await File.ReadAllTextAsync("destination.json");

        if (!string.IsNullOrWhiteSpace(destinationJson))
        {
            destination = JsonSerializer.Deserialize<DestinationKey>(destinationJson);
        }
    }
}

var samConnection = new SamConnection();
await samConnection.ConnectAsync();

var samSession = new SamSession(samConnection);
destination = await samSession.CreatePrimarySessionAsync(destination);

await File.WriteAllTextAsync("destination.json",
    JsonSerializer.Serialize(destination, new JsonSerializerOptions { WriteIndented = true }));

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

    //var client = new StreamClient(samSession, remoteDestination);
    var client = new RawClient(samSession, remoteDestination);
    await client.StartAsync();
}
else
{
    //var server = new StreamServer(samSession);
    var server = new RawServer(samSession);
    await server.StartAsync();
}
