using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace PractiSES
{
    public class HashMAC
    {
        private byte[] secretkey;

        public HashMAC()
        {
            // Create a random key using a random number generator. 
            // This would be the secret key shared by sender and receiver.
            secretkey = new Byte[64];
            // RNGCryptoServiceProvider is an implementation of
            // a random number generator.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            // The array is now filled with random bytes.
            rng.GetBytes(secretkey);
        }

        public HashMAC(string macPassword)
        {
            // Take random key as parameter. 
            secretkey = new Byte[64];
            secretkey = Encoding.Unicode.GetBytes(macPassword);
        }

        public byte[] SecretKey()
        {
            return this.secretkey;
        }

        public string HMAC(string originalText)
        {
            //hashes original text with bytes, need to convert text to bytes 
            byte[] data = Encoding.Unicode.GetBytes(originalText);
            // Initialize the keyed hash object. 
            HMACSHA1 hmacSHA1 = new HMACSHA1(secretkey);
            byte[] macSender = hmacSHA1.ComputeHash(data);
            return macSender.ToString();
        }

        public bool ValidateMAC(string originalText, string mac)
        {
            //hashes original text with bytes, need to convert text to bytes 
            byte[] data = Encoding.Unicode.GetBytes(originalText);
            //convert mac to bytes 
            byte[] macSender = Encoding.Unicode.GetBytes(mac);
            // Initialize the keyed hash object. 
            HMACSHA1 hmacSHA1 = new HMACSHA1(secretkey);
            byte[] macReciever = hmacSHA1.ComputeHash(data);
            bool identical = true;
            // compare the computed hash with the stored value
            for (int i = 0; i < macReciever.Length; i++)
            {
                if (macReciever[i] != macSender[i])
                {
                    identical = false;
                    break;
                }
            }
            return identical;
        }
    }
}
