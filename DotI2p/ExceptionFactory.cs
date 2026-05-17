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

            if (!response.ResponseDictionary.TryGetValue("MESSAGE", out var message))
            {
                message = response.OriginalResponse;
            }

            return result switch
            {
                "OK" => new Exception($"SAM bridge returned OK: {response.OriginalResponse}"),
                "I2P_ERROR" => new I2pErrorException(message),
                "NOVERSION" => new NoVersionException(message),
                "DUPLICATED_ID" => new DuplicatedIdException(message),
                "DUPLICATED_DEST" => new DuplicatedDestException(message),
                "INVALID_KEY" => new InvalidKeyException(message),
                "CANT_REACH_PEER" => new CantReachPeerException(message),
                "INVALID_ID" => new InvalidIdException(message),
                "TIMEOUT" => new TimeoutException(message),
                _ => new Exception($"SAM bridge returned an error: {response.OriginalResponse}"),
            };
        }
    }
}
