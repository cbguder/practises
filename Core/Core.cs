/*
 * $Id$
 */

using System;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

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
            keyFile = Path.Combine(appDataFolder, "private.key");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            this.InitializeKeys();
        }

        private void InitializeKeys()
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA();
            
            Console.Write("Enter passphrase: ");
            String passphrase = Console.ReadLine();
            passphrase.Trim();

            if (!File.Exists(keyFile))
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                WriteKey(keyFile, privateKey, passphrase);
                Console.WriteLine("Public/Private key pair written to " + keyFile);
            }
            else
            {
                String keyString = ReadKey(keyFile, passphrase);
                rsa.FromXmlString(keyString);
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                Console.WriteLine("Public/Private key pair read from " + keyFile);
            }
        }

        private void WriteKey(String path, String key, String passphrase)
        {
            Rijndael aes = Rijndael.Create();
            AESInfo info = Crypto.AESEncrypt(Encoding.ASCII.GetBytes(key), passphrase);
            File.WriteAllText(path, Convert.ToBase64String(Util.Join(info.salt, info.message)));
        }

        private String ReadKey(String path, String passphrase)
        {
            Rijndael aes = Rijndael.Create();
            ArrayList file = new ArrayList(Convert.FromBase64String(File.ReadAllText(path)));
            byte[] salt = (byte [])file.GetRange(0, 8).ToArray(Type.GetType("System.Byte"));
            byte[] rest = (byte[])file.GetRange(8, file.Count - 8).ToArray(Type.GetType("System.Byte"));
            return Encoding.ASCII.GetString(Crypto.AESDecrypt(rest, passphrase, salt));
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
