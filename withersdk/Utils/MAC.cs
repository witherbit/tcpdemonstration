using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace withersdk.Utils
{
    public static class MAC
    {
        public static byte[] ComputeHMAC(this byte[] plain, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(plain);
            }
        }

        public static bool CheckHMAC(this byte[] plain, byte[] key, byte[] mac)
        {
            using (var hmac = new HMACSHA512(key))
            {
                var hash = hmac.ComputeHash(plain);
                return hash.SequenceEqual(mac);
            }
        }
    }
}
