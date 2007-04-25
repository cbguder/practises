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
        private String actionLogFile;
        private String errorLogFile;
        private String appDataFolder;
        private String publicKey;
        private String privateKey;
        private String exeName;
        public const String separator = "--------------------";
        public const String space = ": ";

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

        public String ApplicationDataFolder
        {
            get
            {
                return this.appDataFolder;
            }
        }

        public String KeyFile
        {
            get
            {
                return this.keyFile;
            }
        }

        public String ActionLogFile
        {
            get
            {
                return this.actionLogFile;
            }
        }

        public String ErrorLogFile
        {
            get
            {
                return this.errorLogFile;
            }
        }
        
        public Core() : this(null)
        {
        }

        public Core(String passphrase) : this(passphrase, true)
        {
        }

        public Core(String passphrase, Boolean autoInitialize)
        {
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            if (exeName == "Client.exe")
            {
                appDataFolder = Path.Combine(appDataFolder, "PractiSES\\Client");
            }
            if(exeName == "Server.exe" || exeName == "Server.vshost.exe")
            {
                appDataFolder = Path.Combine(appDataFolder, "PractiSES\\Server");
                settingsFile = Path.Combine(appDataFolder, "settings.xml");
                errorLogFile = Path.Combine(appDataFolder, "error.log");
                actionLogFile = Path.Combine(appDataFolder, "action.log");
                CreateLogFile(errorLogFile);
                CreateLogFile(actionLogFile);
            }
            keyFile = Path.Combine(appDataFolder, "private.key");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            if (autoInitialize)
            {
                this.InitializeKeys(passphrase);
            }
        }

        public void InitializeKeys(String passphrase)
        {
            RSACryptoServiceProvider rsa = Crypto.GetRSA();

            if (passphrase == null)
            {
                Console.Write("Enter passphrase: ");
                passphrase = Console.ReadLine();
                passphrase.Trim();
            }

            if (!File.Exists(keyFile))
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                if (exeName == "Server.exe")
                {
                    WriteKey(keyFile, privateKey, passphrase);
                    StreamWriter writer = new StreamWriter(actionLogFile, true);
                    writer.Write(DateTime.Now.ToString() + space);
                    writer.WriteLine("Public/Private key pair written to " + keyFile);
                    writer.Close();
                }
                Console.Write(DateTime.Now.ToString() + Core.space);
                Console.WriteLine("Public/Private key pair written.");
            }
            else
            {
                String keyString = ReadKey(keyFile, passphrase);
                rsa.FromXmlString(keyString);
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                if (exeName == "Server.exe")
                {
                    StreamWriter writer = new StreamWriter(actionLogFile, true);
                    writer.Write(DateTime.Now.ToString() + space);
                    writer.WriteLine("Public/Private key pair read from " + keyFile);
                    writer.Close();
                }
                Console.Write(DateTime.Now.ToString() + Core.space);
                Console.WriteLine("Public/Private key pair read.");
            }
        }

        private void CreateLogFile(String logFile)
        {
            bool fileExists = true;
            if (!File.Exists(logFile))
            {
                fileExists = false;
            }
            StreamWriter writer = new StreamWriter(logFile, true);
            if (!fileExists)
            {               
                writer.WriteLine(separator);
                writer.Write(DateTime.Now.ToString() + space);
                writer.WriteLine("Log file has been created.");
            }
            writer.WriteLine();
            writer.WriteLine(separator);
            writer.Write(DateTime.Now.ToString() + space);
            writer.WriteLine("Logging started.");
            writer.Close();
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
                XmlElement questionElement;
                XmlAttribute questionNumber;
                XmlDocument settingsDocument;
                XmlProcessingInstruction instruction;

                settingsDocument = new XmlDocument();

                try
                {
                    instruction = settingsDocument.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    settingsDocument.AppendChild(instruction);

                    questionElement = settingsDocument.CreateElement("", "settings", "");
                    settingsDocument.AppendChild(questionElement);

                    questionElement = settingsDocument.CreateElement("", "question", "");
                    questionNumber = settingsDocument.CreateAttribute("one");
                    Console.WriteLine("Please enter asked secret question:");
                    String strQuestion = Console.ReadLine();
                    Console.WriteLine("Thank you!");
                    questionNumber.InnerText = strQuestion;
                    questionElement.Attributes.Append(questionNumber);
                    settingsDocument.ChildNodes.Item(1).AppendChild(questionElement);

                    settingsDocument.Save(settingsFile);
                    Console.WriteLine(settingsFile + " has been created.");
                }
                catch (XmlException e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }
            else
            {
                XmlNode questionNode;
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();
                settingsDocument.Load(settingsFile);

                questionNode = settingsDocument.SelectSingleNode("descendant::question");
                String question = questionNode.Attributes.GetNamedItem("one").Value;

                StreamWriter writer = new StreamWriter(actionLogFile, true);
                writer.Write(DateTime.Now.ToString() + space);
                writer.WriteLine("'" + question + "'" + " has been read from " + settingsFile);
                writer.Close();
                Console.Write(DateTime.Now.ToString() + Core.space);
                Console.WriteLine("'" + question + "'" + " has been read.");

                return question;             
            }
        }

    }
}
