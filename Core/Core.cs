/*
 * $Id$
 */

using System;
using System.IO;
using System.Xml;

namespace PractiSES
{
    public class Core
    {
        public Encryption encryption;
        private String keyFile;
        private String settingsPath;
        private String appDataFolder;

        public Core()
        {
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appDataFolder, "PractiSES");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        public void ReadWriteKeyFile()
        {
            keyFile = Path.Combine(appDataFolder, "key.xml");

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

        public string ReadQuestionsFromSettingsFile()
        {
            settingsPath = Path.Combine(appDataFolder, "settings.xml");

            if (!File.Exists(settingsPath))
            {
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();

                settingsDocument.CreateNode(XmlNodeType.Element, "settings", settingsPath.ToString());
                settingsDocument.CreateNode(XmlNodeType.EndElement, "settings", settingsPath.ToString());

                /*encryption = new Encryption();

                StreamWriter settingsWriter = new StreamWriter(settingsFile);
                String xmlString = encryption.ToXmlString(true);
                settingsWriter.Write(xmlString);
                settingsWriter.Close();*/
                Console.WriteLine(settingsPath + " has been created. No question has been found.");
                return null;
            }
            else
            {
                XmlNode questionNode;
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();
                settingsDocument.Load(settingsPath);

                questionNode = settingsDocument.SelectSingleNode("descendant::question");
                Console.WriteLine(questionNode.Attributes.GetNamedItem("one").Value + " has been read from " + settingsPath);
                return questionNode.Attributes.GetNamedItem("one").Value;

                /*StreamReader settingsReader = new StreamReader(settingsFile);
                String xmlString = settingsReader.ReadToEnd();
                settingsReader.Close();
                encryption = new Encryption(xmlString);*/
                
            }
        }

    }
}
