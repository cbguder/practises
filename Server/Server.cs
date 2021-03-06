using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;


namespace PractiSES
{
    class Server
    {
        public static String passphrase;
        private IRootServer rootServer;

        private bool Connect(String host)
        {
            Console.WriteLine("Connecting to PractiSES root server ({0})...", host);
            rootServer = (IRootServer)Activator.GetObject(typeof(IRootServer), "http://" + host + ":88/PractiSES_Root");
            try
            {
                if (rootServer.Hello())
                {
                    Console.WriteLine("Connected to PractiSES root server.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Unable to connect to PractiSES root server.");
                return false;
            }

            /*Console.Write("Domain Name: ");
            String domainName = Console.ReadLine();
            GetCertificate(domainName);*/

            return true;
        }

        /*private void GetCertificate(String domainName)
        {
            //String cert = rootServer.GetCertificate(domainName);
            //String[] certFields = cert.Split(',');
            //Console.WriteLine(cert);
            byte[] rawCertData = rootServer.GetCertificate(domainName);
            if (rawCertData != null)
            {
                Certificate.OpenCertificate();
                Certificate.AddCertificate(rawCertData);
                //Console.WriteLine(Convert.ToBase64String(rawCertData));
                Console.WriteLine("Certificate has been downloaded successfully.");
            }
        }*/

        static void Main(string[] args)
        {
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

            ServerObject serverobj = new ServerObject();
            //serverobj.KeyObt("cbguder@su.sabanciuniv.edu", DateTime.Now);
            

            Console.Write("Enter passphrase: ");
            passphrase = Console.ReadLine();
            passphrase.Trim();
            Core core = new Core(passphrase);
            core.ReadSettingsFile();

            Server server = new Server();
            server.Connect(core.GetXmlNodeInnerText("root_server"));

            DatabaseConnection connection = new DatabaseConnection();
            String publicKey = core.PublicKey;
            String dbPublicKey = connection.getPublicKey("server");
            connection.close();
            StreamWriter writer = new StreamWriter(core.ActionLogFile, true);        
            if (publicKey != dbPublicKey)
            {
                writer.Write(DateTime.Now.ToString() + Core.space);
                writer.WriteLine("Server's old public key:");
                writer.WriteLine();
                writer.WriteLine(dbPublicKey);
                writer.WriteLine();

                //connection = new DatabaseConnection();
                //connection.setPublicKey("server", "server", publicKey);
                connection.updatePublicKey("server", "server", publicKey);
                Console.Write(DateTime.Now.ToString() + Core.space);
                Console.WriteLine("New key pair is set.");
            }
            connection.close();
            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine("Server's public key:");
            writer.WriteLine();
            writer.WriteLine(publicKey);
            writer.WriteLine();

            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine("Server started");
            writer.Close();

            Console.Write(DateTime.Now.ToString() + Core.space);
            Console.WriteLine("PractiSES Server started.");


            HttpServerChannel channel = new HttpServerChannel(80);
            ChannelServices.RegisterChannel(channel,false);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PractiSES.ServerObject),
                "PractiSES",
                WellKnownObjectMode.SingleCall);
            System.Console.ReadLine();
        }
    }
}
