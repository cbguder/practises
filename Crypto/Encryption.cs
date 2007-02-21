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
            byte[] data = Encoding.UTF32.GetBytes(secretKey);
            StringBuilder stringBuilder = new StringBuilder();
            byte[] encryptedBytes = rsaCrypto.Encrypt(data, true);
            // Be aware the RSACryptoServiceProvider reverses the order of 
            // encrypted bytes. It does this after encryption and before 
            // decryption. If you do not require compatibility with Microsoft 
            // Cryptographic API (CAPI) and/or other vendors. Comment out the 
            // next line and the corresponding one in the DecryptString function.
            Array.Reverse(encryptedBytes);
            // Why convert to base 64?
            // Because it is the largest power-of-two base printable using only 
            // ASCII characters
            stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            return stringBuilder.ToString();
        }
    }
}
