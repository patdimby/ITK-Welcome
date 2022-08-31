using System;
using Intitek.Welcome.Infrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HelperUT
{
    [TestClass]
    public class EnryptionHelperUT
    {
        [TestMethod]
        public void EncryptAndDecrypt()
        {
            string toEncrypt = "fyb8DkymTtiDPOFAC9eT1XWhDhhpVjx7mblJbRQdiCmi3D4FsQCDRGM9kYeh5M+a";
            string key = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M=";
            string iv = "qf6bYB7dJxer+CQjoVhAdQ==";

            string encryptedValue = EncryptionHelper.Encrypt(toEncrypt, key, iv);

            System.Diagnostics.Debug.WriteLine($"EncryptedValue: {encryptedValue}");
            System.Diagnostics.Debug.WriteLine($"Key: {key}");
            System.Diagnostics.Debug.WriteLine($"Iv: {iv}");

            string decryptedValue = EncryptionHelper.Decrypt(encryptedValue, key, iv);

            System.Diagnostics.Debug.WriteLine($"DecryptedValue: {decryptedValue}");

            Assert.AreEqual(toEncrypt, decryptedValue);
        }
    }
}
