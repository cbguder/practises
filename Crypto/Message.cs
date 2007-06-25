/*
 * $Id$
 */

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

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
            get { return cleartext; }
        }

        public byte[] Ciphertext
        {
            get { return ciphertext; }
        }

        public byte[] Signature
        {
            get { return signature; }
            set { signature = value; }
        }

        public DateTime Time
        {
            get { return time; }
        }

        public Message()
        {
            time = DateTime.Now;
            cleartext = null;
            ciphertext = null;
            signature = null;
            comments = new ArrayList();
        }

        public Message(String message) : this()
        {
            String[] lines = Util.GetLines(message);

            int firstLine = 0;
            while (lines[firstLine] == "")
                firstLine++;

            int i = firstLine + 1;

            if (lines[firstLine] == Crypto.BeginMessage || lines[firstLine] == Crypto.BeginSignedMessage)
            {
                while (lines[i] != "")
                {
                    String[] commentParts = lines[i].Split(new String[] {": "}, StringSplitOptions.None);
                    AddComment(commentParts[0], commentParts[1]);
                    i++;
                }
            }

            if (lines[firstLine] == Crypto.BeginMessage)
            {
                int startIndex, endIndex;

                startIndex = i + 1;
                while (lines[i] != Crypto.EndMessage)
                    i++;
                endIndex = i;

                ciphertext = Convert.FromBase64String(String.Join("", lines, startIndex, endIndex - startIndex));
            }
            else if (lines[firstLine] == Crypto.BeginSignedMessage)
            {
                int startIndex, endIndex;

                startIndex = i + 1;
                while (lines[i] != Crypto.BeginSignature)
                    i++;
                endIndex = i;

                cleartext =
                    Encoding.UTF8.GetBytes(String.Join("\n", lines, startIndex, endIndex - startIndex));

                startIndex = endIndex + 1;
                while (lines[i] != Crypto.EndSignature)
                    i++;
                endIndex = i;

                signature = Convert.FromBase64String(String.Join("", lines, startIndex, endIndex - startIndex));
            }
            else
            {
                cleartext = Encoding.UTF8.GetBytes(message);
                comments.Add(
                    new Comment("Version",
                                "PractiSES " + Assembly.GetExecutingAssembly().GetName().Version.ToString(2) +
                                " (Win32)"));
                comments.Add(new Comment("Time", time.ToUniversalTime().ToString("u")));
            }
        }

        public Message(byte[] message) : this()
        {
            cleartext = message;
            comments.Add(
                new Comment("Version",
                            "PractiSES " + Assembly.GetExecutingAssembly().GetName().Version.ToString(2) + " (Win32)"));
            comments.Add(new Comment("Time", time.ToUniversalTime().ToString("u")));
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

            comments.Add(new Comment(name, content));
        }

        public void Encrypt(String publicKey)
        {
            ciphertext = Crypto.RAWEncrypt(cleartext, publicKey);
        }

        public void Decrypt(String privateKey)
        {
            Message temp = new Message(Crypto.RAWDecrypt(ciphertext, privateKey));
            cleartext = temp.cleartext;
            signature = temp.signature;
            comments = temp.comments;
        }

        public void Sign(String privateKey)
        {
            Sign(privateKey, true);
        }

        public void Sign(String privateKey, bool includeComments)
        {
            byte[] toSign;

            if (includeComments)
            {
                byte[] commentBytes = Encoding.UTF8.GetBytes(getComments());
                toSign = Util.Join(commentBytes, cleartext);
            }
            else
            {
                toSign = cleartext;
            }
                        
            signature = Crypto.Sign(toSign, privateKey);
        }

        public bool Verify(String publicKey)
        {
            return Verify(publicKey, true);
        }

        public bool Verify(String publicKey, bool includeComments)
        {
            byte[] toVerify;

            if (includeComments)
            {
                byte[] commentBytes = Encoding.UTF8.GetBytes(getComments());
                toVerify = Util.Join(commentBytes, cleartext);
            }
            else
            {
                toVerify = cleartext;
            }

            return Crypto.Verify(toVerify, signature, publicKey);
        }

        public override String ToString()
        {
            StringWriter result = new StringWriter();
            result.NewLine = "\n";

            if (ciphertext != null)
            {
                result.WriteLine(Crypto.BeginMessage);
                result.WriteLine(getComments());
                result.WriteLine(Util.Wrap(Convert.ToBase64String(ciphertext), wrap));
                result.WriteLine(Crypto.EndMessage);
            }
            else if (signature != null)
            {
                result.WriteLine(Crypto.BeginSignedMessage);
                result.WriteLine(getComments());
                result.WriteLine(getCleartext());
                result.WriteLine(getSignature());
            }
            else
            {
                result.WriteLine(getCleartext());
            }

            result.Flush();
            return result.ToString();
        }

        public String getSignature()
        {
            String result = Crypto.BeginSignature;
            result += "\n";
            result += Util.Wrap(Convert.ToBase64String(signature), wrap);
            result += "\n";
            result += Crypto.EndSignature;
            return result;
        }

        public String getComments()
        {
            String result = "";
            foreach (Comment c in comments)
            {
                result += c.name + ": " + c.content;
                result += "\n";
            }
            return result;
        }

        public String getCleartext()
        {
            return Encoding.UTF8.GetString(cleartext);
        }
    }
}