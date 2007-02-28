/*
 * $Id$
 */

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Security.Cryptography;
using System.IO;
using System.Net;

namespace PractiSES
{
    class Client
    {
        private const String host = "10.90.12.89";

        static void Main(string[] args)
        {
            Encryption encryption;

            String keyFile;
            String appDataFolder;

            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appDataFolder, "PractiSES");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            keyFile = Path.Combine(appDataFolder, "key.xml");

            if (!File.Exists(keyFile))
            {
                encryption = new Encryption();

                StreamWriter keyWriter = new StreamWriter(keyFile);
                String xmlString = encryption.ToXmlString(true);
                keyWriter.Write(xmlString);
                keyWriter.Close();
            }
            else
            {
                StreamReader keyReader = new StreamReader(keyFile);
                String xmlString = keyReader.ReadToEnd();
                keyReader.Close();
                encryption = new Encryption(xmlString);
                Console.WriteLine("Public/Private key pair read from " + keyFile);
            }

            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.WriteLine("Connecting to PractiSES server ({0})...", host);

            ServerObject server = (ServerObject)Activator.GetObject(typeof(PractiSES.ServerObject), "http://" + host + "/PractiSES");

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            try
            {
                Console.WriteLine(server.KeyObt("cbguder@su.sabanciuniv.edu"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.ReadLine();
        }
    }
}
