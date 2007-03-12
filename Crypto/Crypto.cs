/*
 * $Id$
 */

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace PractiSES
{
	public static class Crypto
	{
        private static int RSAKeySize = 2048;
        private static int AESKeySize = 256;
        private static int AESIVSize = 128;

        public static RSACryptoServiceProvider GetRSA()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSAKeySize);
            return rsa;
        }

        public static RSACryptoServiceProvider GetRSA(String key)
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA();
            rsa.FromXmlString(key);
            return rsa;
        }

        public static String Sign(String clearText, String privateKey)
        {
            String signedMessage = "";

            signedMessage += "-----BEGIN PRACTISES SIGNED MESSAGE-----";
            signedMessage += Environment.NewLine;
            signedMessage += clearText;
            signedMessage += Environment.NewLine;
            signedMessage += "-----BEGIN PRACTISES SIGNATURE-----";
            signedMessage += Environment.NewLine;
            signedMessage += Util.Wrap(Crypto.RSAGetSignature(clearText, privateKey), 64);
            signedMessage += Environment.NewLine;
            signedMessage += "-----END PRACTISES SIGNATURE-----";
            signedMessage += Environment.NewLine;

            return signedMessage;
        }

        public static String Encrypt(String clearText, String publicKey)
        {
            String cipherText = "";

            cipherText += "-----BEGIN PRACTISES MESSAGE-----";
            cipherText += Environment.NewLine;
            cipherText += Util.Wrap(Crypto.RSAEncrypt(clearText, publicKey), 64);
            cipherText += Environment.NewLine;
            cipherText += "-----END PRACTISES MESSAGE-----";
            cipherText += Environment.NewLine;

            return cipherText;
        }

        public static String SignAndEncrypt(String clearText, String publicKey, String privateKey)
        {
            String signedMessage = Crypto.Sign(clearText, privateKey);
            return Crypto.Encrypt(signedMessage, publicKey);
        }

        private static String RSAEncrypt(String clearText, String publicKey)
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA(publicKey);
            
            int keySize = Crypto.RSAKeySize / 8;
            byte[] bytes = Encoding.UTF8.GetBytes(clearText);
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder StringBuilder = new StringBuilder();

            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsa.Encrypt(tempBytes, true);
                StringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }

            return StringBuilder.ToString();
        }

        private static String RSADecrypt(String cipherText, String privateKey)
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA(privateKey);

            int base64BlockSize = ((RSAKeySize / 8) % 3 != 0) ?
              (((RSAKeySize / 8) / 3) * 4) + 4 : ((RSAKeySize / 8) / 3) * 4;

            int iterations = cipherText.Length / base64BlockSize;

            ArrayList arrayList = new ArrayList();

            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(cipherText.Substring(base64BlockSize * i, base64BlockSize));
                arrayList.AddRange(rsa.Decrypt(encryptedBytes, true));
            }

            return Encoding.UTF8.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }

        private static String RSAGetSignature(String clearText, String privateKey)
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA(privateKey);

            byte[] bytes = Encoding.UTF8.GetBytes(clearText);
            byte[] signature = rsa.SignData(bytes, new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signature);
        }

        private static AESInfo DeriveKeyAndIV(String passphrase, byte[] salt, int keyLen, int IVLen)
        {
            const int saltLength = 8;
            int k = keyLen / 8;
            int i = IVLen / 8;

            AESInfo result = new AESInfo();

            byte[] passphraseBytes = Encoding.UTF8.GetBytes(passphrase);

            if (salt == null)
            {
                result.salt = new byte[saltLength];
                Random random = new Random();
                random.NextBytes(result.salt);
            }
            else
            {
                result.salt = salt;
            }

            ArrayList keyAndIV = new ArrayList(k + i);
            keyAndIV.AddRange(PBKDF2(passphraseBytes, result.salt, 10000, k + i));

            result.key = (byte[])keyAndIV.GetRange(0, k).ToArray(Type.GetType("System.Byte"));
            result.IV = (byte[])keyAndIV.GetRange(k, i).ToArray(Type.GetType("System.Byte"));

            return result;
        }
        
        /*
         * PBKDF2 as described in PKCS #5 v2.0 pp.8-10
         */
        private static byte[] PBKDF2(byte[] passphrase, byte[] salt, int c, int dkLen)
        {
            const int hLen = 20;
            int l = (int)Math.Ceiling((double)dkLen / hLen);
            int r = dkLen - (l - 1) * hLen;

            ArrayList result = new ArrayList(dkLen);

            for (int i = 0; i < l; i++)
            {
                result.AddRange(F(passphrase, salt, c, i));
            }

            return (byte [])result.GetRange(0, dkLen).ToArray(System.Type.GetType("System.Byte"));
        }

        /*
         * F as described in PKCS #5 v2.0 p.9
         */
        private static byte[] F(byte[] passphrase, byte[] salt, int c, int i)
        {
            HMACSHA1 hmac = new HMACSHA1(passphrase);

            byte[] result = hmac.ComputeHash(Util.Join(salt, BitConverter.GetBytes(i)));

            for(int j = 1; j < c; j++)
            {
                result = Util.XOR(result, hmac.ComputeHash(result));
            }           

            return result;
        }

        public static String AESEncrypt(String clearText, String passphrase)
        {
            MemoryStream memoryStream = new MemoryStream();
            
            Rijndael aes = Rijndael.Create();

            AESInfo aesInfo = Crypto.DeriveKeyAndIV(passphrase, null, Crypto.AESKeySize, Crypto.AESIVSize);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(aesInfo.key, aesInfo.IV), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);

            streamWriter.Write(clearText);

            streamWriter.Close();
            cryptoStream.Close();

            String result = Convert.ToBase64String(aesInfo.salt);
            result += Environment.NewLine;
            result += Convert.ToBase64String(memoryStream.ToArray());

            memoryStream.Close();

            return result;
        }

        public static string AESDecrypt(String cipherText, String passphrase)
        {
            StringReader stringReader = new StringReader(cipherText);
            byte[] salt = Convert.FromBase64String(stringReader.ReadLine());
            
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(stringReader.ReadToEnd()));

            Rijndael aes = Rijndael.Create();

            AESInfo aesInfo = Crypto.DeriveKeyAndIV(passphrase, salt, Crypto.AESKeySize, Crypto.AESIVSize);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(aesInfo.key, aesInfo.IV), CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);

            String result = null;

            result = streamReader.ReadToEnd();

            return result;
        }
	}
}
