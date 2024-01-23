using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods;
using withersdk.Net.Methods.Wrapper;

namespace withersdk.Utils
{
    public static class NetUtils
    {
        public static string ConvertToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static byte[] ConvertToBytes(this string @string)
        {
            return Encoding.UTF8.GetBytes(@string);
        }
        public static byte[] GenerateKey(this int count)
        {
            byte[] bytes = new byte[count];
            RNGCryptoServiceProvider.Create().GetBytes(bytes);
            return bytes;
        }

        public static (byte[] PublicKey, byte[] Signature) SignItOLD(this byte[] message, byte[] signKey)
        {
            var publicKey = Ed25519old.PublicKey(signKey);
            return (publicKey, Ed25519old.Signature(message, signKey, publicKey));
        }
        public static bool VerifySignOLD(this byte[] signature, byte[] message, byte[] publicKey)
        {
            return Ed25519old.CheckValid(signature, message, publicKey);
        }

        public static (byte[] PublicKey, byte[] Signature) SignIt(this byte[] message, byte[] signKey)
        {
            Ed25519.KeyPairFromSeed(out byte[] publicKey, out byte[] privateKey, signKey);
            return (publicKey, Ed25519.Sign(message, privateKey));
        }
        public static bool VerifySign(this byte[] signature, byte[] message, byte[] publicKey)
        {
            return Ed25519.Verify(signature, message, publicKey);
        }

        public static byte[] Concat(this byte[] arr, byte[] arr2)
        {
            return arr.Concat(arr2).ToArray();
        }

        public static string GetMessageType(this byte[] message)
        {
            var o = JObject.Parse(Encoding.UTF8.GetString(message));
            return o["Type"].ToString();
        }
        public static string GetMethodType(this string method)
        {
            try
            {
                var o = JObject.Parse(method);
                return o["Type"].ToString();
            }
            catch
            {
                return null;
            }
        }
        public static string GetMethodFieldValue(this string method, string field)
        {
            var o = JObject.Parse(method);
            return o[field].ToString();
        }

        public static string GetMessageFieldValue(this byte[] message, string field)
        {
            var o = JObject.Parse(Encoding.UTF8.GetString(message));
            return o[field].ToString();
        }

        public static string ToConsoleOutputString(this SignedNode msg)
        {
            return 
                $"\tType: {msg.Type}\n" +
                $"\tMessage: {msg.Content}\n" +
                $"\tBody: {msg.Body.ToBase64String()}\n" +
                $"\tMAC: {msg.MAC.ToBase64String()}\n" +
                $"\tSignature: {msg.Sign.ToBase64String()}\n" +
                $"\tSignature public key: {msg.SignPublicKey.ToBase64String()}\n";
        }
        public static string ToConsoleOutputString(this Node msg)
        {
            return
                $"\tType: {msg.Type}\n" +
                $"\tMessage: {msg.Content}\n" +
                $"\tBody: {msg.Body.ToBase64String()}\n" +
                $"\tMAC: {msg.MAC.ToBase64String()}\n";
        }

        public static (double RequestMs, double ResponseMs, double TotalMs) GetPingMilliseconds(this RequestMethod.Ping ping, ResponseMethod.Pong pong, DateTime responseGetTime)
        {
            return ((pong.GetTime - ping.SendTime).TotalMilliseconds, (responseGetTime - pong.SendTime).TotalMilliseconds, (responseGetTime - ping.SendTime).TotalMilliseconds);
        }
    }
}
