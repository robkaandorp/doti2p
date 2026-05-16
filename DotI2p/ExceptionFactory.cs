using System;

namespace DotI2p
{
    public class I2pErrorException : Exception
    {
        public I2pErrorException(string message) : base(message) { }
    }

    public class NoVersionException : Exception
    {
        public NoVersionException(string message) : base(message) { }
    }

    public class DuplicatedIdException : Exception
    {
        public DuplicatedIdException(string message) : base(message) { }
    }

    public class DuplicatedDestException : Exception
    {
        public DuplicatedDestException(string message) : base(message) { }
    }

    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message) : base(message) { }
    }

    public class CantReachPeerException : Exception
    {
        public CantReachPeerException(string message) : base(message) { }
    }

    public class InvalidIdException : Exception
    {
        public InvalidIdException(string message) : base(message) { }
    }

    public class TimeoutException : Exception
    {
        public TimeoutException(string message) : base(message) { }
    }

    public static class ExceptionFactory
    {
        public static Exception Create(CommandResponse response)
        {
            if (!response.ResponseDictionary.TryGetValue("RESULT", out var result))
            {
                return new Exception($"Unexpected response: {response.OriginalResponse}");
            }

            return result switch
            {
                "OK" => new Exception($"SAM bridge returned OK: {response.OriginalResponse}"),
                "I2P_ERROR" => new I2pErrorException(response.ResponseDictionary["MESSAGE"]),
                "NOVERSION" => new NoVersionException(response.ResponseDictionary["MESSAGE"]),
                "DUPLICATED_ID" => new DuplicatedIdException(response.ResponseDictionary["MESSAGE"]),
                "DUPLICATED_DEST" => new DuplicatedDestException(response.ResponseDictionary["MESSAGE"]),
                "INVALID_KEY" => new InvalidKeyException(response.ResponseDictionary["MESSAGE"]),
                "CANT_REACH_PEER" => new CantReachPeerException(response.ResponseDictionary["MESSAGE"]),
                "INVALID_ID" => new InvalidIdException(response.ResponseDictionary["MESSAGE"]),
                "TIMEOUT" => new TimeoutException(response.ResponseDictionary["MESSAGE"]),
                _ => new Exception($"SAM bridge returned an error: {response.OriginalResponse}"),
            };
        }
    }
}
