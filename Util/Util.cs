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
    }
}
