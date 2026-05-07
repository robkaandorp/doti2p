# DotI2p

A .NET library for communicating with an [I2Pd](https://i2pd.website/) router via the [SAM v3.1 protocol](https://geti2p.net/en/docs/api/samv3). It lets you create anonymous streaming connections over the I2P network from any .NET application.

## Prerequisites

- An [I2Pd](https://i2pd.website/) router running with the SAM interface enabled (default port `7656`)
- .NET SDK 6.0 or later (the library targets `netstandard2.1`)

## Installation

Add a project reference to `DotI2p`:

```shell
dotnet add reference path/to/DotI2p/DotI2p.csproj
```

## Quick Start

```csharp
using DotI2p;

// Connect to the local SAM bridge
var connection = new SamConnection(); // defaults to 127.0.0.1:7656
await connection.ConnectAsync();

// Create a session and generate a destination
var session = new SamSession(connection);
var destination = await session.CreateStreamAsync();

Console.WriteLine($"Listening on: {destination.GetB32Hostname()}");
```

## Usage

### Connecting to a Remote Destination

```csharp
// Look up a .b32.i2p hostname
var remote = await session.HostNameLookupAsync("something.b32.i2p");

// Open a virtual stream and connect
var virtualStream = session.CreateVirtualStream();
var tcpClient = await virtualStream.ConnectAsync(remote);

// Use the TcpClient's NetworkStream for I/O
var stream = tcpClient.GetStream();
var writer = new StreamWriter(stream);
var reader = new StreamReader(stream);

writer.WriteLine("Hello over I2P!");
writer.Flush();
```

### Accepting Incoming Connections

```csharp
var virtualStream = session.CreateVirtualStream();
var accepted = await virtualStream.AcceptAsync();

Console.WriteLine($"Connection from: {accepted.Destination.GetB32Hostname()}");

var stream = accepted.TcpClient.GetStream();
// Read/write on stream...
```

### Custom SAM Bridge Address

```csharp
using System.Net;

var connection = new SamConnection(IPAddress.Parse("192.168.1.100"), port: 7656);
await connection.ConnectAsync();
```

## API Overview

| Class | Purpose |
|-------|---------|
| `SamConnection` | Manages the TCP connection to the SAM bridge and handles the handshake and command I/O |
| `SamSession` | Creates sessions, generates destination keys, and performs hostname lookups |
| `SamVirtualStream` | Opens stream connections (`STREAM CONNECT`) or accepts incoming ones (`STREAM ACCEPT`) |
| `DestinationKey` | Wraps an I2P destination (public + optional private key); can derive `.b32.i2p` hostnames |
| `CommandResponse` | Parses SAM protocol responses into a response type and key-value dictionary |
| `ExceptionFactory` | Maps SAM error codes (`CANT_REACH_PEER`, `TIMEOUT`, etc.) to typed exceptions |

## Building from Source

```shell
dotnet build doti2p.slnx
```

### Running the Sample Client

The included `SampleClient` project demonstrates both client and server modes:

```shell
# Server mode — listens for incoming connections
dotnet run --project SampleClient

# Client mode — connects to a remote destination
dotnet run --project SampleClient -- <destination-or-hostname.b32.i2p>
```

## License

[MIT](LICENSE)
