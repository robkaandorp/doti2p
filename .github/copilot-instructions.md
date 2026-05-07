# Copilot Instructions

## Build Commands

```shell
# Build the solution
dotnet build doti2p.slnx

# Run the sample client (requires a running I2Pd instance with SAM enabled on port 7656)
dotnet run --project SampleClient
```

There is no test project in this repository.

## Architecture

DotI2p is a .NET library implementing the [I2P SAM v3.1 protocol](https://geti2p.net/en/docs/api/samv3) to communicate with an I2Pd router. The flow is layered:

1. **SamConnection** – TCP socket to the SAM bridge (default `127.0.0.1:7656`). Handles the `HELLO` handshake and raw command/response I/O.
2. **SamSession** – Creates a session (`SESSION CREATE`), generates destination keys (`DEST GENERATE`), and performs hostname lookups (`NAMING LOOKUP`).
3. **SamVirtualStream** – Opens a new `SamConnection` clone per stream to issue `STREAM CONNECT` or `STREAM ACCEPT`, then hands back the underlying `TcpClient` for application-level I/O.

Supporting types:
- **CommandResponse** – Parses SAM responses: first two words are the response type, remaining tokens are `KEY=VALUE` pairs.
- **DestinationKey** – Wraps a public (and optional private) I2P destination key; can derive `.b32.i2p` hostnames via SHA-256 + Base32.
- **ExceptionFactory** – Maps SAM `RESULT` codes to typed exceptions.

## Conventions

- Library targets **netstandard2.1**; sample client targets **net10.0**.
- Solution uses the `.slnx` format.
- **Nullable** reference types are enabled in all projects.
- **TreatWarningsAsErrors** is enabled for both Debug and Release.
- Instance members are accessed with explicit `this.` prefix.
- The library uses explicit `using` statements (no implicit usings).
- Async methods follow the `*Async` suffix convention and return `Task`/`Task<T>`.
