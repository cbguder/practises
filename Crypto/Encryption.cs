/*
 * $Id$
 */

using System;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace PractiSES
{
	public class Encryption
	{
        private RSACryptoServiceProvider rsa;
        private const int dwKeySize = 2048;

        public Encryption()
        {
            rsa = new RSACryptoServiceProvider(dwKeySize);
        }

        public Encryption(String xmlString) : this()
        {
            rsa.FromXmlString(xmlString);
        }

        public String ToXmlString(bool includePrivateParameters)
        {
            return rsa.ToXmlString(includePrivateParameters);
        }

		public String EncryptString(String inputString)
		{
			// TODO: Add Proper Exception Handlers

			int keySize = dwKeySize / 8;
			byte[] bytes = Encoding.UTF32.GetBytes(inputString);
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

		public String DecryptString(String inputString)
		{
			// TODO: Add Proper Exception Handlers

			int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
			  (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;

			int iterations = inputString.Length / base64BlockSize;

			ArrayList arrayList = new ArrayList();

			for (int i = 0; i < iterations; i++)
			{
				byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                arrayList.AddRange(rsa.Decrypt(encryptedBytes, true));
			}

			return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
		}
	}
}
