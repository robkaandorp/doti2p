using System.Collections.Generic;
using System.Linq;

namespace DotI2p
{
    public class CommandResponse
    {
        public string OriginalResponse { get; }

        public string Response { get; }

        public IDictionary<string, string> ResponseDictionary { get; }

        public CommandResponse(string response)
        {
            this.OriginalResponse = response;

            var parts = this.OriginalResponse.Split(' ');

            this.Response = string.Join(" ", parts.Take(2));
            this.ResponseDictionary = parts.Skip(2)
                .Select(part => part.Split('='))
                .ToDictionary(pair => pair[0], pair => pair.Length == 1 ? pair[0] : pair[1]);
        }
    }
}
