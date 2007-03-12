/*
 * $Id$
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public static class Util
    {
        public static String Wrap(String input, int maxLength)
        {
            int index = 0;
            String result = "";

            while (index < input.Length)
            {
                if (maxLength < input.Length - index)
                {
                    result += input.Substring(index, maxLength);
                    result += Environment.NewLine;
                    index += maxLength;
                }
                else
                {
                    result += input.Substring(index);
                    index = input.Length;
                }
            }

            return result;
        }

        public static byte[] Join(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            a.CopyTo(result, 0);
            b.CopyTo(result, a.Length);
            return result;
        }

        public static byte[] XOR(byte[] a, byte[] b)
        {
            int min = Math.Min(a.Length, b.Length);

            byte[] result = new Byte[min];

            for (int i = 0; i < min; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }

            return result;
        }
    }
}
