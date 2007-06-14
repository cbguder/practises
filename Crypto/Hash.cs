using System;
using System.Security.Cryptography;
using System.Text;

namespace PractiSES
{
    public class Hash
    {
        private byte[] secretkey;

        public byte[] SecretKey
        {
            get { return secretkey; }
        }

        public Hash()
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

        public Hash(string macPassword)
        {
            // Take random key as parameter. 
            secretkey = new Byte[64];
            secretkey = Encoding.UTF8.GetBytes(macPassword);
        }

        public string HMAC(string originalText)
        {
            //hashes original text with bytes, need to convert text to bytes 
            byte[] data = Encoding.UTF8.GetBytes(originalText);
            // Initialize the keyed hash object. 
            HMACSHA1 hmacSHA1 = new HMACSHA1(secretkey);

            byte[] macSender = hmacSHA1.ComputeHash(data);
            return Convert.ToBase64String(macSender);
        }

        public bool ValidateMAC(string originalText, string mac)
        {
            //hashes original text with bytes, need to convert text to bytes 
            byte[] data = Encoding.UTF8.GetBytes(originalText);
            //convert mac to bytes 
            byte[] macSender = Encoding.UTF8.GetBytes(mac);
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