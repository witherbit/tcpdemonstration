using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace withersdk.Utils
{
    public static class Rfc2898Util
    {
        public const int defaultSaltSize = 16;
        public const int defaultKeySize = 16;
        public const int defaultIterations = 10000;

        public static string Rfc2898Derive(this string plainPassword, int saltSize = defaultSaltSize, int iterations = defaultIterations, int keySize = defaultKeySize)
        {
            using (var derive = new Rfc2898DeriveBytes(plainPassword, saltSize: saltSize, iterations: iterations))
            {
                var b64Pwd = Convert.ToBase64String(derive.GetBytes(keySize));
                var b64Salt = Convert.ToBase64String(derive.Salt);
                return string.Join(":", b64Salt, iterations.ToString(), keySize.ToString(), b64Pwd);
            }
        }
        public static bool Rfc2898Verify(this string saltedPassword, string plainPassword)
        {

            var passwordParts = saltedPassword.Split(':');
            var salt = Convert.FromBase64String(passwordParts[0]);
            var iters = int.Parse(passwordParts[1]);
            var keySize = int.Parse(passwordParts[2]);
            var pwd = Convert.FromBase64String(passwordParts[3]);
            using (var derive = new Rfc2898DeriveBytes(plainPassword, salt: salt, iterations: iters))
            {
                var hashedInput = derive.GetBytes(keySize);

                return hashedInput.SequenceEqual(pwd);
            }
        }

        public static string Rfc2898DeriveInline(this string plainPassword)
        {
            using (var derive = new Rfc2898DeriveBytes(plainPassword, saltSize: 32, iterations: 65536))
            {
                var b64Pwd = Convert.ToBase64String(derive.GetBytes(32));
                var b64Salt = Convert.ToBase64String(derive.Salt);
                return string.Join("", b64Salt.Remove(b64Salt.Length-1).Insert(b64Salt.Length-1, "A"), b64Pwd);
            }
        }
        public static bool Rfc2898VerifyInline(this string saltedPassword, string plainPassword)
        {
            var passwordParts0 = saltedPassword.Remove(43).Insert(43, "=");
            var passwordParts1 = saltedPassword.Remove(0, 44);
            var salt = Convert.FromBase64String(passwordParts0);
            var pwd = Convert.FromBase64String(passwordParts1);
            using (var derive = new Rfc2898DeriveBytes(plainPassword, salt: salt, iterations: 65536))
            {
                var hashedInput = derive.GetBytes(32);

                return hashedInput.SequenceEqual(pwd);
            }
        }

        public static byte[] Rfc2898DeriveInlineBytes(this string plainPassword)
        {
            using (var derive = new Rfc2898DeriveBytes(plainPassword, saltSize: 32, iterations: 65536))
            {
                var b64Pwd = Convert.ToBase64String(derive.GetBytes(32));
                var b64Salt = Convert.ToBase64String(derive.Salt);
                return string.Join("", b64Salt.Remove(b64Salt.Length - 1).Insert(b64Salt.Length - 1, "A"), b64Pwd).FromUTF8();
            }
        }

        public static bool Rfc2898VerifyInlineBytes(this byte[] saltedPassword, string plainPassword)
        {
            var passwordParts0 = saltedPassword.ToUTF8().Remove(43).Insert(43, "=");
            var passwordParts1 = saltedPassword.ToUTF8().Remove(0, 44);
            var salt = Convert.FromBase64String(passwordParts0);
            var pwd = Convert.FromBase64String(passwordParts1);
            using (var derive = new Rfc2898DeriveBytes(plainPassword, salt: salt, iterations: 65536))
            {
                var hashedInput = derive.GetBytes(32);

                return hashedInput.SequenceEqual(pwd);
            }
        }
    }
}
