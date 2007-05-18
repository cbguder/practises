using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public class Message
    {
        private String cleartext;
        private byte[] ciphertext;
        private byte[] signature;

        public String Cleartext
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
        }

        public Message() : this(null, null, null)
        {
        }

        public Message(String cleartext, byte[] ciphertext, byte[] signature)
        {
            this.cleartext = cleartext;
            this.ciphertext = ciphertext;
            this.signature = signature;
        }
    }
}
