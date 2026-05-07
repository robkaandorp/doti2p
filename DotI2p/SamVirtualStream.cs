using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotI2p
{
    public class SamVirtualStream
    {
        private readonly SamConnection connection;
        private readonly string sessionId;

        public SamVirtualStream(SamConnection connection, string sessionId)
        {
            this.connection = connection;
            this.sessionId = sessionId;
        }

        public async Task<TcpClient> ConnectAsync(DestinationKey destination)
        {
            await this.connection.ConnectAsync();

            var response = await connection.SendCommandAsync($"STREAM CONNECT ID={this.sessionId} DESTINATION={destination.Destination}");

            if (!response.Response.Equals("STREAM STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            return this.connection.GetTcpClient();
        }

        public async Task<AcceptedConnection> AcceptAsync()
        {
            await this.connection.ConnectAsync();

            var response = await connection.SendCommandAsync($"STREAM ACCEPT ID={this.sessionId}");

            if (!response.Response.Equals("STREAM STATUS", StringComparison.Ordinal))
            {
                throw new Exception($"Unexpected response from SAM bridge: {response.OriginalResponse}");
            }

            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result) || !result.Equals("OK", StringComparison.Ordinal))
            {
                throw ExceptionFactory.Create(response);
            }

            var acceptResponse = await this.connection.ReadLineAsync();

            if (acceptResponse == null)
            {
                throw new Exception($"Unexpected null response from SAM bridge.");
            }

            if (acceptResponse.StartsWith("STREAM STATUS"))
            {
                var acceptCommandResponse = new CommandResponse(acceptResponse);

                throw ExceptionFactory.Create(acceptCommandResponse);
            }

            return new AcceptedConnection(new DestinationKey(acceptResponse.Split(' ')[0]), this.connection.GetTcpClient());
        }
    }
}
