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

        public byte[] GetPubKeyHash()
        {
            using var sha256 = SHA256.Create();
            var pubKey = Convert.FromBase64String(this.Destination.Replace('-', '+').Replace('~', '/'));

            return sha256.ComputeHash(pubKey);
        }

        public string GetB32Hostname() =>
            $"{Multibase.Encode(MultibaseEncoding.Base32Lower, this.GetPubKeyHash())[1..]}.b32.i2p";
    }
}
