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
        private String keyFile;
        private String settingsFile;
        private String appDataFolder;

        public Core()
        {
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appDataFolder, "PractiSES");

            keyFile = Path.Combine(appDataFolder, "key.xml");
            settingsFile = Path.Combine(appDataFolder, "settings.xml");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            if (!File.Exists(keyFile))
            {
                encryption = new Encryption();

                StreamWriter keyWriter = new StreamWriter(keyFile);
                String xmlString = encryption.ToXmlString(true);
                keyWriter.Write(xmlString);
                keyWriter.Close();
                Console.WriteLine("Public/Private key pair written to " + keyFile);
            }
            else
            {
                StreamReader keyReader = new StreamReader(keyFile);
                String xmlString = keyReader.ReadToEnd();
                keyReader.Close();
                encryption = new Encryption(xmlString);
                Console.WriteLine("Public/Private key pair read from " + keyFile);
            }
        }
    }
}
