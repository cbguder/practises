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

        private DateTime time;
        private byte[] cleartext;
        private byte[] ciphertext;
        private byte[] signature;
        private ArrayList comments;

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

        public DateTime Time
        {
            get
            {
                return this.time;
            }
        }

        public Message()
        {
            this.time = DateTime.Now;
            this.cleartext = null;
            this.ciphertext = null;
            this.signature = null;
            this.comments = new ArrayList();
            comments.Add(new Comment("Version", "PractiSES " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2)+ " (Win32)"));
            comments.Add(new Comment("Time", this.time.ToUniversalTime().ToString("u")));
        }

        public Message(String message) : this()
        {
            String[] lines = Util.GetLines(message);
            int i = 1;

            if (lines[0] == Crypto.BeginMessage || lines[0] == Crypto.BeginSignedMessage)
            {
                while (lines[i] != "")
                {
                    String[] commentParts = lines[i].Split(new String[] { ": " }, StringSplitOptions.None);
                    this.AddComment(commentParts[0], commentParts[1]);
                    i++;
                }
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

        public void AddComment(String name, String content)
        {
            foreach (Comment c in comments)
            {
                if (c.name == name)
                {
                    c.content = content;
                    return;
                }
            }

            this.comments.Add(new Comment(name, content));
        }

        public void Encrypt(String publicKey)
        {
            this.ciphertext = Crypto.RAWEncrypt(this.cleartext, publicKey);
        }

        public void Decrypt(String privateKey)
        {
            Message temp = new Message(Crypto.RAWDecrypt(this.ciphertext, privateKey));
            this.cleartext = temp.cleartext;
            this.signature = temp.signature;
            this.comments = temp.comments;
        }

        public void Sign(String privateKey)
        {
            byte[] commentBytes = Encoding.UTF8.GetBytes(this.getComments());
            this.signature = Crypto.Sign(Util.Join(commentBytes, cleartext), privateKey);
        }

        public bool Verify(String publicKey)
        {
            byte[] commentBytes = Encoding.UTF8.GetBytes(this.getComments());
            return Crypto.Verify(Util.Join(commentBytes, cleartext), signature, publicKey);
        }

        public override String ToString()
        {
            StringWriter result = new StringWriter();

            if (this.ciphertext != null)
            {
                result.WriteLine(Crypto.BeginMessage);
                result.WriteLine(this.getComments());
                result.WriteLine(Util.Wrap(Convert.ToBase64String(ciphertext), wrap));
                result.WriteLine(Crypto.EndMessage);
            }
            else if (this.signature != null)
            {
                result.WriteLine(Crypto.BeginSignedMessage);
                result.WriteLine(this.getComments());
                result.WriteLine(this.getCleartext());
                result.WriteLine(this.getSignature());
            }
            else
            {
                result.WriteLine(this.getCleartext());
            }

            result.Flush();
            return result.ToString();
        }

        public String getSignature()
        {
            String result = Crypto.BeginSignature;
            result += Environment.NewLine;
            result += Util.Wrap(Convert.ToBase64String(signature), wrap);
            result += Environment.NewLine;
            result += Crypto.EndSignature;
            return result;
        }

        public String getComments()
        {
            String result = "";
            foreach (Comment c in this.comments)
            {
                result += c.name + ": " + c.content;
                result += Environment.NewLine;
            }
            return result;
        }

        public String getCleartext()
        {
            return Encoding.UTF8.GetString(cleartext);
        }
    }
}
