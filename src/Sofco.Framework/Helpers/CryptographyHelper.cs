using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sofco.Framework.Helpers
{
    public static class CryptographyHelper
    {
        private static readonly string Key = "GoMiRPMrAamuDezt";
        private static readonly byte[] Iv = { 35, 0, 102, 0, 97, 0, 102, 0, 97, 0, 102, 0, 97, 0, 102, 0 };

        public static string Encrypt(string text)
        {
            var inputbuffer = Encoding.UTF8.GetBytes(text);
            var key = Encoding.Unicode.GetBytes(Key);
            var transform = Algorithm.CreateEncryptor(key, Iv);
            var outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        private static SymmetricAlgorithm Algorithm => Aes.Create();

        public static string Decrypt(string text)
        {
            if (text == null)
                return string.Empty;
            var inputbuffer = Convert.FromBase64String(text);
            var key = Encoding.Unicode.GetBytes(Key);
            var transform = Algorithm.CreateDecryptor(key, Iv);
            var outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.UTF8.GetString(outputBuffer);
        }
    }
}
