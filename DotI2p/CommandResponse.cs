using System.Collections.Generic;
using System.Text;

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

            var firstSpace = response.IndexOf(' ');
            var secondSpace = firstSpace >= 0 ? response.IndexOf(' ', firstSpace + 1) : -1;

            if (secondSpace >= 0)
            {
                this.Response = response.Substring(0, secondSpace);
                this.ResponseDictionary = ParseKeyValuePairs(response, secondSpace + 1);
            }
            else
            {
                this.Response = response;
                this.ResponseDictionary = new Dictionary<string, string>();
            }
        }

        private static Dictionary<string, string> ParseKeyValuePairs(string input, int startIndex)
        {
            var dictionary = new Dictionary<string, string>();
            var key = new StringBuilder();
            var value = new StringBuilder();
            var inQuotes = false;
            var parsingValue = false;

            for (var i = startIndex; i <= input.Length; i++)
            {
                // Treat end-of-string as a space to flush the last pair.
                var c = i < input.Length ? input[i] : ' ';

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == '=' && !parsingValue && !inQuotes)
                {
                    parsingValue = true;
                }
                else if (c == ' ' && !inQuotes)
                {
                    if (key.Length > 0)
                    {
                        dictionary[key.ToString()] = value.ToString();
                        key.Clear();
                        value.Clear();
                    }

                    parsingValue = false;
                }
                else
                {
                    if (parsingValue)
                    {
                        value.Append(c);
                    }
                    else
                    {
                        key.Append(c);
                    }
                }
            }

            return dictionary;
        }
    }
}
