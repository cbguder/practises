/*
 * $Id$
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

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
        private String processName;
        public const String separator = "--------------------";
        public const String space = ": ";

        public String PublicKey
        {
            get { return publicKey; }
        }

        public String PrivateKey
        {
            get { return privateKey; }
        }

        public String ApplicationDataFolder
        {
            get { return appDataFolder; }
        }

        public String KeyFile
        {
            get { return keyFile; }
        }

        public String ActionLogFile
        {
            get { return actionLogFile; }
        }

        public String ErrorLogFile
        {
            get { return errorLogFile; }
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
            processName = Process.GetCurrentProcess().ProcessName;

            if (processName == "Client" || processName == "PractiSES" || processName == "Client.vshost")
            {
                appDataFolder = Path.Combine(appDataFolder, "PractiSES\\Client");
            }
            if (processName == "Server" || processName == "Server.vshost")
            {
                appDataFolder = Path.Combine(appDataFolder, "PractiSES\\Server");
                settingsFile = Path.Combine(appDataFolder, "settings.xml");
                errorLogFile = Path.Combine(appDataFolder, "error.log");
                actionLogFile = Path.Combine(appDataFolder, "action.log");
            }
            keyFile = Path.Combine(appDataFolder, "private.key");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            if (processName == "Server" || processName == "Server.vshost")
            {
                CreateLogFile(errorLogFile);
                CreateLogFile(actionLogFile);
            }

            if (autoInitialize)
            {
                InitializeKeys(passphrase);
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
                WriteKey(keyFile, privateKey, passphrase);
                if (processName == "Server" || processName == "Server.vshost")
                {
                    StreamWriter writer = new StreamWriter(actionLogFile, true);
                    writer.Write(DateTime.Now + space);
                    writer.WriteLine("Public/Private key pair written to " + keyFile);
                    writer.Close();
                }
                Console.Write(DateTime.Now + space);
                Console.WriteLine("Public/Private key pair written.");
            }
            else
            {
                String keyString = ReadKey(keyFile, passphrase);
                rsa.FromXmlString(keyString);
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                if (processName == "Server" || processName == "Server.vshost")
                {
                    StreamWriter writer = new StreamWriter(actionLogFile, true);
                    writer.Write(DateTime.Now + space);
                    writer.WriteLine("Public/Private key pair read from " + keyFile);
                    writer.Close();
                }
                Console.Write(DateTime.Now + space);
                Console.WriteLine("Public/Private key pair read.");
            }
        }

        private static void CreateLogFile(String logFile)
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
                writer.Write(DateTime.Now + space);
                writer.WriteLine("Log file has been created.");
            }
            writer.WriteLine();
            writer.WriteLine(separator);
            writer.Write(DateTime.Now + space);
            writer.WriteLine("Logging started.");
            writer.Close();
        }

        private static void WriteKey(String path, String key, String passphrase)
        {
            AESInfo info = Crypto.AESEncrypt(Encoding.ASCII.GetBytes(key), passphrase);
            File.WriteAllText(path, Convert.ToBase64String(Util.Join(info.salt, info.message)));
        }

        private static String ReadKey(String path, String passphrase)
        {
            ArrayList file = new ArrayList(Convert.FromBase64String(File.ReadAllText(path)));
            byte[] salt = (byte[]) file.GetRange(0, 8).ToArray(Type.GetType("System.Byte"));
            byte[] rest = (byte[]) file.GetRange(8, file.Count - 8).ToArray(Type.GetType("System.Byte"));
            return Encoding.ASCII.GetString(Crypto.AESDecrypt(rest, passphrase, salt));
        }

        public String ReadSettingsFile()
        {
            if (!File.Exists(settingsFile))
            {
                XmlDocument settingsDocument;

                settingsDocument = new XmlDocument();

                try
                {
                    XmlProcessingInstruction instruction = settingsDocument.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    settingsDocument.AppendChild(instruction);

                    XmlElement settingsElement = settingsDocument.CreateElement("", "settings", "");
                    settingsDocument.AppendChild(settingsElement);

                    XmlElement questionElement = settingsDocument.CreateElement("", "question", "");
                    XmlAttribute questionNumber;
                    questionNumber = settingsDocument.CreateAttribute("one");
                    Console.WriteLine("Please enter asked secret question:");
                    String strQuestion = Console.ReadLine();
                    Console.WriteLine("Thank you!");
                    questionNumber.InnerText = strQuestion;
                    questionElement.Attributes.Append(questionNumber);
                    settingsDocument.ChildNodes.Item(1).AppendChild(questionElement);

                    XmlElement domainElement;
                    domainElement = settingsDocument.CreateElement("", "domain", "");
                    Console.WriteLine("Please enter domain name of the server:");
                    String domainName = Console.ReadLine();
                    Console.WriteLine("Thank you again!");
                    if (domainName[1] != '@')
                    {
                        domainName = "@" + domainName;
                    }
                    domainElement.InnerText = domainName;
                    settingsDocument.ChildNodes.Item(1).AppendChild(domainElement);


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
                writer.Write(DateTime.Now + space);
                writer.WriteLine("'" + question + "'" + " has been read from " + settingsFile);
                writer.Close();
                Console.Write(DateTime.Now + space);
                Console.WriteLine("'" + question + "'" + " has been read.");

                return question;
            }
        }

        public String GetDomainName()
        {
            XmlNode domainNode;
            XmlDocument settingsDocument;

            settingsDocument = new XmlDocument();
            settingsDocument.Load(settingsFile);

            domainNode = settingsDocument.SelectSingleNode("descendant::domain");
            String domain = domainNode.InnerText;

            return domain;
        }
    }
}