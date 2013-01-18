using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace RemoteAppLauncher.Infrastructure.Utilities
{
    internal static class HashUtility
    {
        public static string Md5FromString(string toHash)
        {
            byte[] valueBytes = Encoding.ASCII.GetBytes(toHash);
            return Md5HashFromBytes(valueBytes);
        }

        public static string Md5HashFromBytes(byte[] toHash)
        {
            var hashProvider = new MD5CryptoServiceProvider();
            hashProvider.ComputeHash(toHash);
            return StringFromHashedBytes(hashProvider.Hash);
        }

        public static string Md5HashFromStream(Stream toHash)
        {
            var hashProvider = new MD5CryptoServiceProvider();
            hashProvider.ComputeHash(toHash);
            return StringFromHashedBytes(hashProvider.Hash);
        }

        private static string StringFromHashedBytes(byte[] hashBytes)
        {
            if(hashBytes == null || hashBytes.Length == 0)
                throw new ArgumentException("hash");

            var hashBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashBuilder.Append(hashBytes[i].ToString("x2"));
            }
            return hashBuilder.ToString();
        }
    }
}
