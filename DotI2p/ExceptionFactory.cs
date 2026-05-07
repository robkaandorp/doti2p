using System;

namespace DotI2p
{
    public class I2pErrorException : Exception
    {
        public I2pErrorException(string message) : base(message) { }
    }

    public class NoVersionException : Exception { }

    public class DuplicatedIdException : Exception { }

    public class DuplicatedDestException : Exception { }

    public class InvalidKeyException : Exception { }

    public class CantReachPeerException : Exception { }

    public class InvalidIdException : Exception { }
    
    public class TimeoutException : Exception { }

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
                "NOVERSION" => new NoVersionException(),
                "DUPLICATED_ID" => new DuplicatedIdException(),
                "DUPLICATED_DEST" => new DuplicatedDestException(),
                "INVALID_KEY" => new InvalidKeyException(),
                "CANT_REACH_PEER" => new CantReachPeerException(),
                "INVALID_ID" => new InvalidIdException(),
                "TIMEOUT" => new TimeoutException(),
                _ => new Exception($"SAM bridge returned an error: {response.OriginalResponse}"),
            };
        }
    }
}
