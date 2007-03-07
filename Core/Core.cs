/*
 * $Id$
 */

using System;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

namespace PractiSES
{
    public class Core
    {
        private String keyFile;
        private String settingsFile;
        private String appDataFolder;
        private String publicKey;
        private String privateKey;

        public String PublicKey
        {
            get
            {
                return this.publicKey;
            }
        }

        public String PrivateKey
        {
            get
            {
                return this.privateKey;
            }
        }
        
        public Core()
        {
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appDataFolder, "PractiSES");
            settingsFile = Path.Combine(appDataFolder, "settings.xml");
            keyFile = Path.Combine(appDataFolder, "key.xml");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            this.ReadKey();
        }

        public void ReadKey()
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA();

            if (!File.Exists(keyFile))
            {
                StreamWriter keyWriter = new StreamWriter(keyFile);
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                keyWriter.Write(privateKey);
                keyWriter.Close();
                Console.WriteLine("Public/Private key pair written to " + keyFile);
            }
            else
            {
                StreamReader keyReader = new StreamReader(keyFile);
                String keyString = keyReader.ReadToEnd();
                rsa.FromXmlString(keyString);
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                Console.WriteLine("Public/Private key pair read from " + keyFile);
            }
        }

        public String ReadQuestions()
        {
            if (!File.Exists(settingsFile))
            {
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();

                settingsDocument.CreateNode(XmlNodeType.Element, "settings", settingsFile.ToString());
                settingsDocument.CreateNode(XmlNodeType.EndElement, "settings", settingsFile.ToString());

                /*encryption = new Encryption();

                StreamWriter settingsWriter = new StreamWriter(settingsFile);
                String xmlString = encryption.ToXmlString(true);
                settingsWriter.Write(xmlString);
                settingsWriter.Close();*/
                Console.WriteLine(settingsFile + " has been created. No question has been found.");
                return null;
            }
            else
            {
                XmlNode questionNode;
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();
                settingsDocument.Load(settingsFile);

                questionNode = settingsDocument.SelectSingleNode("descendant::question");
                Console.WriteLine(questionNode.Attributes.GetNamedItem("one").Value + " has been read from " + settingsFile);
                return questionNode.Attributes.GetNamedItem("one").Value;

                /*StreamReader settingsReader = new StreamReader(settingsFile);
                String xmlString = settingsReader.ReadToEnd();
                settingsReader.Close();
                encryption = new Encryption(xmlString);*/
                
            }
        }

    }
}
