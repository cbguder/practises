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

        public static bool Compare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static String[][] Getopt(String[] args, String options)
        {
            return Util.Getopt(args, options, null);
        }

        public static String[][] Getopt(String[] args, String options, String[] longOptions)
        {
            ArrayList result = new ArrayList();
            String toSearch;
            bool needsParameter;

            for (int i = 0; i < options.Length; i++)
            {
                toSearch = "-" + options[i];
                needsParameter = (i < options.Length - 1) && (options[i + 1] == ':');

                for(int j = 0; j < args.Length; j++)
                {
                    if(args[j] == toSearch && !needsParameter)
                    {
                        result.Add(new String[] { toSearch, "" });
                    }
                    else if (args[j].StartsWith(toSearch) && needsParameter)
                    {
                        String[] item = new String[] { toSearch, "" };

                        if (args[j].Length > 2)
                        {
                            item[1] = args[j].Substring(2);
                        }
                        else if (j < args.Length - 1)
                        {
                            item[1] = args[j + 1];
                            j++;
                        }

                        result.Add(item);
                        i++;
                    }
                }
            }

            if (longOptions != null)
            {
                foreach (String longOption in longOptions)
                {
                    needsParameter = longOption.EndsWith("=");

                    foreach (String arg in args)
                    {
                        if (arg == longOption && !needsParameter)
                        {
                            result.Add(new String[] { longOption, "" });                            
                        }
                        else if (arg.StartsWith(longOption) && needsParameter)
                        {
                            result.Add(arg.Split('='));
                        }
                    }
                }
            }

            return (String[][])result.ToArray(Type.GetType("System.String[]"));
        }

        public static String[] GetLines(String input)
        {
            ArrayList lines = new ArrayList();
            StringReader sr = new StringReader(input);
            String line;

            while (true)
            {
                line = sr.ReadLine();
                
                if (line == null)
                    break;

                lines.Add(line);
            }

            return (String[])lines.ToArray(Type.GetType("System.String"));
        }
    }
}
