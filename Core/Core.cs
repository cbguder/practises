/*
 * $Id$
 */

using System;
using System.IO;

namespace PractiSES
{
    public class Core
    {
        public Encryption encryption;
        public String keyFilePath;

        public Core()
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appDataFolder, "PractiSES");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            keyFilePath = Path.Combine(appDataFolder, "key.xml");

            if (!File.Exists(keyFilePath))
            {
                encryption = new Encryption();

                StreamWriter keyWriter = new StreamWriter(keyFilePath);
                String xmlString = encryption.ToXmlString(true);
                keyWriter.Write(xmlString);
                keyWriter.Close();
                Console.WriteLine("Public/Private key pair written to " + keyFilePath);
            }
            else
            {
                StreamReader keyReader = new StreamReader(keyFilePath);
                String xmlString = keyReader.ReadToEnd();
                keyReader.Close();
                encryption = new Encryption(xmlString);
                Console.WriteLine("Public/Private key pair read from " + keyFilePath);
            }
        }
    }
}
