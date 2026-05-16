# DotI2p

A .NET library for communicating with an [I2Pd](https://i2pd.website/) router via the [SAM v3.3 protocol](https://geti2p.net/en/docs/api/samv3). It lets you create anonymous streaming and raw (UDP) connections over the I2P network from any .NET application.

## Prerequisites

- An [I2Pd](https://i2pd.website/) router running with the SAM interface enabled (default TCP port `7656`, default UDP port `7655`)
- .NET SDK 6.0 or later (the library targets `netstandard2.1`)

## Installation

```shell
dotnet add package DotI2p
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

### Primary Sessions with Sub-Sessions (SAM 3.3)

A primary (master) session lets you create multiple stream and raw sub-sessions on a single destination:

```csharp
var connection = new SamConnection();
await connection.ConnectAsync();

var session = new SamSession(connection);
var destination = await session.CreatePrimarySessionAsync();

// Create a stream sub-session for TCP-like connections
var streamSubSession = await session.CreateStreamSubSession();
var virtualStream = streamSubSession.CreateVirtualStream();

// Create a raw sub-session for UDP-like datagrams
var rawSubSession = await session.CreateRawSubSession(new RawSubSessionConfiguration
{
    Port = 12345
});
```

### Sending and Receiving Raw (UDP) Datagrams

```csharp
// Send a datagram to a remote destination
await rawSubSession.SendAsync(
    remoteDestination.PubKey,
    fromPort: null,
    toPort: null,
    protocol: null,
    data: System.Text.Encoding.UTF8.GetBytes("Hello via UDP!")
);

// Receive a datagram
var data = await rawSubSession.ReceiveAsync();
```

### Custom SAM Bridge Address

```csharp
using System.Net;

var connection = new SamConnection(
    IPAddress.Parse("192.168.1.100"),
    tcpPort: 7656,
    udpPort: 7655
);
await connection.ConnectAsync();
```

## API Overview

| Class | Purpose |
|-------|---------|
| `SamConnection` | Manages the TCP/UDP connection to the SAM bridge and handles the handshake and command I/O |
| `SamSession` | Creates sessions (stream or primary), generates destination keys, and performs hostname lookups |
| `SamVirtualStream` | Opens stream connections (`STREAM CONNECT`) or accepts incoming ones (`STREAM ACCEPT`) |
| `SamStreamSubSession` | A stream sub-session created from a primary session; provides `CreateVirtualStream()` |
| `SamRawSubSession` | A raw (UDP) sub-session for sending and receiving datagrams via the I2P network |
| `RawSubSessionConfiguration` | Configuration for creating a raw sub-session (port, host, protocol filters) |
| `DestinationKey` | Wraps an I2P destination (public + optional private key); can derive `.b32.i2p` hostnames |
| `AcceptedConnection` | Represents an accepted inbound stream connection (destination + `TcpClient`) |
| `CommandResponse` | Parses SAM protocol responses into a response type and key-value dictionary |
| `ExceptionFactory` | Maps SAM error codes (`CANT_REACH_PEER`, `TIMEOUT`, etc.) to typed exceptions |

## Building from Source

```shell
dotnet build doti2p.slnx
```

### Running the Tests

```shell
dotnet test DotI2p.Tests
```

### Running the Sample Client

The included `SampleClient` project demonstrates streaming and raw (UDP) modes, each with a client and server:

```shell
# Server mode — listens for incoming connections
dotnet run --project SampleClient

# Client mode — connects to a remote destination
dotnet run --project SampleClient -- <destination-or-hostname.b32.i2p>
```

## License

[MIT](LICENSE)
