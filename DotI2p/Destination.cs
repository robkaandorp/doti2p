using Multiformats.Base;

using System;
using System.Security.Cryptography;

namespace DotI2p
{
    public class DestinationKey
    {
        public string Destination { get; }
        public string? PrivKey { get; }

        public DestinationKey(string destination, string? privkey = null)
        {
            this.Destination = destination;
            this.PrivKey = privkey;
        }

        public string GetB32Hostname()
        {
            var pubKey = Convert.FromBase64String(this.Destination.Replace('-', '+').Replace('~', '/'));

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(pubKey);

            return $"{Multibase.Encode(MultibaseEncoding.Base32Lower, hash)[1..]}.b32.i2p";
        }
    }
}
