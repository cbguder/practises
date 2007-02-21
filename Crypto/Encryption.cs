using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace PractiSES
{
    public class Encryption
    {
        public string EncryptSK(string secretKey, int dwKeySize)
        {
            RSACryptoServiceProvider rsaCrypto = new RSACryptoServiceProvider(dwKeySize);
            byte[] data = Encoding.UTF32.GetBytes(secretKey);;
            string encryptedText = rsaCrypto.Encrypt(data, true);
            return encryptedText;
        }
    }
}
