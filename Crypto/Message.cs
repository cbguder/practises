/*
 * $Id$
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace PractiSES
{
    public class Message
    {
        private const int wrap = 64;

        private byte[] cleartext;
        private byte[] ciphertext;
        private byte[] signature;

        public byte[] Cleartext
        {
            get
            {
                return this.cleartext;
            }
        }

        public byte[] Ciphertext
        {
            get
            {
                return this.ciphertext;
            }
        }

        public byte[] Signature
        {
            get
            {
                return this.signature;
            }
            set
            {
                this.signature = value;
            }
        }

        public Message()
        {
            this.cleartext = null;
            this.ciphertext = null;
            this.signature = null;
        }

        public Message(String message) : this()
        {
            String[] lines = Util.GetLines(message);
            int i = 1;

            if (lines[0] == Crypto.BeginMessage || lines[0] == Crypto.BeginSignedMessage)
            {
                while (lines[i] != "")
                    i++;
            }

            if (lines[0] == Crypto.BeginMessage)
            {
                int startIndex, endIndex;

                startIndex = i + 1;
                while (lines[i] != Crypto.EndMessage)
                    i++;
                endIndex = i;

                this.ciphertext = Convert.FromBase64String(String.Join("", lines, startIndex, endIndex - startIndex));
            }
            else if (lines[0] == Crypto.BeginSignedMessage)
            {
                int startIndex, endIndex;
                
                startIndex = i + 1;
                while (lines[i] != Crypto.BeginSignature)
                    i++;
                endIndex = i;

                this.cleartext = Encoding.UTF8.GetBytes(String.Join(Environment.NewLine, lines, startIndex, endIndex - startIndex));

                startIndex = endIndex + 1;
                while (lines[i] != Crypto.EndSignature)
                    i++;
                endIndex = i;

                this.signature = Convert.FromBase64String(String.Join("", lines, startIndex, endIndex - startIndex));
            }
            else
            {
                this.cleartext = Encoding.UTF8.GetBytes(message);
            }
        }

        public Message(byte[] message) : this()
        {
            this.cleartext = message;
        }

        public void Encrypt()
        {
        }

        public void Decrypt()
        {
        }

        public void Sign(String privateKey)
        {
            this.signature = Crypto.Sign(this.cleartext, privateKey);
        }

        public bool Verify(String publicKey)
        {
            return Crypto.Verify(cleartext, signature, publicKey);
        }

        public override String ToString()
        {
            StringWriter result = new StringWriter();

            if (this.ciphertext != null)
            {
                result.WriteLine(Crypto.BeginMessage);
                result.WriteLine("Version: PractiSES {0} (Win32)", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2));
                result.WriteLine();
                result.WriteLine(Util.Wrap(Convert.ToBase64String(ciphertext), wrap));
                result.WriteLine(Crypto.EndMessage);
            }
            else if (this.signature != null)
            {
                result.WriteLine(Crypto.BeginSignedMessage);
                result.WriteLine("Version: PractiSES {0} (Win32)", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2));
                result.WriteLine();
                result.WriteLine(Encoding.UTF8.GetString(cleartext));
                result.WriteLine(Crypto.BeginSignature);
                result.WriteLine(Util.Wrap(Convert.ToBase64String(signature), wrap));
                result.WriteLine(Crypto.EndSignature);
            }
            else
            {
                result.WriteLine(Encoding.UTF8.GetString(cleartext));
            }

            result.Flush();
            return result.ToString();
        }
    }
}
