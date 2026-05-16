using System;
using System.Net;
using System.Threading.Tasks;

namespace DotI2p
{
    public class SamSession
    {
        private readonly SamConnection connection;

        public string Id { get; } = Guid.NewGuid().ToString("N");

        public DestinationKey? Destination { get; private set; }

        public SamSession(SamConnection connection)
        {
            this.connection = connection;
        }

        public async Task<DestinationKey> GenerateDestinationKeyAsync()
        {
            var response = await this.connection.SendCommandAsync($"DEST GENERATE SIGNATURE_TYPE=7");

            if (!response.Response.Equals("DEST REPLY", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.ContainsKey("PUB"))
            {
                throw new Exception($"PUB not found: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.ContainsKey("PRIV"))
            {
                throw new Exception($"PRIV not found: {response.OriginalResponse}");
            }

            return new DestinationKey(response.ResponseDictionary["PUB"], response.ResponseDictionary["PRIV"]);
        }

        public async Task<DestinationKey> CreateStreamAsync()
        {
            if (this.Destination != null)
            {
                throw new InvalidOperationException("Session already created.");
            }

            var destination = await this.GenerateDestinationKeyAsync();
            var response = await this.connection.SendCommandAsync($"SESSION CREATE STYLE=STREAM ID={this.Id} DESTINATION={destination.PrivKey} i2cp.leaseSetEncType=6,4,0");

            if (!response.Response.Equals("SESSION STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            this.Destination = destination;
            return destination;
        }

        public async Task<DestinationKey> HostNameLookupAsync(string hostname)
        {
            var response = await this.connection.SendCommandAsync($"NAMING LOOKUP NAME={hostname}");

            if (!response.Response.Equals("NAMING REPLY", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            return new DestinationKey(response.ResponseDictionary["VALUE"]);
        }

        public SamVirtualStream CreateVirtualStream()
        {
            return new SamVirtualStream(this.connection.CreateClone(), this.Id);
        }

        public async Task<DestinationKey> CreatePrimarySessionAsync()
        {
            if (this.Destination != null)
            {
                throw new InvalidOperationException("Session already created.");
            }

            var destination = await this.GenerateDestinationKeyAsync();
            var response = await this.connection.SendCommandAsync($"SESSION CREATE STYLE=MASTER ID={this.Id} DESTINATION={destination.PrivKey} i2cp.leaseSetEncType=6,4");

            if (!response.Response.Equals("SESSION STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            this.Destination = destination;
            return destination;
        }

        public async Task<SamStreamSubSession> CreateStreamSubSession()
        {
            var subSessionId = Guid.NewGuid().ToString("N");

            var response = await this.connection.SendCommandAsync($"SESSION ADD STYLE=STREAM ID={subSessionId}");

            if (!response.Response.Equals("SESSION STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            return new SamStreamSubSession(this.connection, subSessionId);
        }

        public async Task<SamRawSubSession> CreateRawSubSession(RawSubSessionConfiguration config)
        {
            var subSessionId = Guid.NewGuid().ToString("N");
            var command = $"SESSION ADD STYLE=RAW ID={subSessionId} PORT={config.Port}";

            if (config.Host != null)
            {
                command += $" HOST={config.Host}";
            }

            if (config.FromPort != null)
            {
                command += $" FROM_PORT={config.FromPort}";
            }

            if (config.ToPort != null)
            {
                command += $" TO_PORT={config.ToPort}";
            }

            if (config.Protocol != null)
            {
                command += $" PROTOCOL={config.Protocol}";
            }

            if (config.ListenPort != null)
            {
                command += $" LISTEN_PORT={config.ListenPort}";
            }

            if (config.ListenProtocol != null)
            {
                command += $" LISTEN_PROTOCOL={config.ListenProtocol}";
            }

            var response = await this.connection.SendCommandAsync(command);

            if (!response.Response.Equals("SESSION STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            return new SamRawSubSession(subSessionId, new IPEndPoint(IPAddress.Any, config.Port), new IPEndPoint(this.connection.Host, this.connection.UdpPort));
        }
    }
}
