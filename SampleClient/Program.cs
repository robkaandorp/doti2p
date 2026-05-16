using DotI2p;

using SampleClient;

var samConnection = new SamConnection();
await samConnection.ConnectAsync();

var samSession = new SamSession(samConnection);
var destination = await samSession.CreatePrimarySessionAsync();

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
