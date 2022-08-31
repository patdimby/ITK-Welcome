using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Config
{
    public static class ConfigurationEncryption
    {
        private const string EncryptionKey = "MAKV2SPBNI99212";
        private const string initVector = "tu89geji340t89u2";
        private const int keysize = 256;

        public static string EncryptConfig(string plainText)/////to encrypt password
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(EncryptionKey, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);////To encrypt
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string DecryptConfig(string cipherText)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(EncryptionKey, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static void EncryptConfigurationFile(string configurationFileName, string encryptedFileName)
        {
            if(!File.Exists(configurationFileName))
            {
                throw new FileNotFoundException("Aucun fichier de configuration disponible.");
            }

            var fileConfig = File.ReadAllText(configurationFileName);
            var configtext = fileConfig.Split(';');

            var config = new Config()
            {
                DBServer = ConfigurationEncryption.EncryptConfig(configtext[0].Split('=')[1]),
                DBName = ConfigurationEncryption.EncryptConfig(configtext[1].Split('=')[1]),
                DBUserID = ConfigurationEncryption.EncryptConfig(configtext[2].Split('=')[1]),
                DBPassword = ConfigurationEncryption.EncryptConfig(configtext[3].Split('=')[1]),
                SMTPUser = ConfigurationEncryption.EncryptConfig(configtext[4].Split('=')[1]),
                SMTPPassword = ConfigurationEncryption.EncryptConfig(configtext[5].Split('=')[1])

            };
            config.Serialize(encryptedFileName);
            File.Delete(configurationFileName);
        }
    }
}
