using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public class EncryptionHelper
    {
        public static string Encrypt(string text, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            using (var rijAlg = new RijndaelManaged())
            {
                byte[] encrypted;
                var encryptor = rijAlg.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(iv));
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }

                return Convert.ToBase64String(encrypted.ToArray());
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string Decrypt(string cipherText, string key, string iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            string text;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Convert.FromBase64String(key);
                rijAlg.IV = Convert.FromBase64String(iv);

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                var cipher = Convert.FromBase64String(cipherText);

                MemoryStream msDecrypt = null;
                CryptoStream csDecrypt = null;
                StreamReader srDecrypt = null;
                try
                {
                    msDecrypt = new MemoryStream(cipher);
                    csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    srDecrypt = new StreamReader(csDecrypt);
                    text = srDecrypt.ReadToEnd();
                }
                finally
                {
                    msDecrypt?.Dispose();
                    csDecrypt?.Dispose();
                    srDecrypt?.Dispose();
                }
            }

            return text;
        }

        public static string CreatePasswordHash(string pwd)
        {
            return CreatePasswordHash(pwd, CreateSalt());
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd = GetHashString(saltAndPwd);
            var saltPosition = 5;
            hashedPwd = hashedPwd.Insert(saltPosition, salt);
            return hashedPwd;
        }

        private static string GetHashString(string password)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(password))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static byte[] GetHash(string password)
        {
            SHA384 sha = new SHA384CryptoServiceProvider();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[20];
            rng.GetBytes(buff);
            var saltSize = 10;
            string salt = Convert.ToBase64String(buff);
            if (salt.Length > saltSize)
            {
                salt = salt.Substring(0, saltSize);
                return salt.ToUpper();
            }

            var saltChar = '^';
            salt = salt.PadRight(saltSize, saltChar);
            return salt.ToUpper();
        }

        public static bool Validate(string password, string passwordHash)
        {
            var saltPosition = 5;
            var saltSize = 10;
            var salt = passwordHash.Substring(saltPosition, saltSize);
            var hashedPassword = CreatePasswordHash(password, salt);
            return hashedPassword == passwordHash;
        }
    }
}
