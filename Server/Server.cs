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

        static void Main(string[] args)
        {
            Console.Write("Enter passphrase: ");
            passphrase = Console.ReadLine();
            passphrase.Trim();
            Core core = new Core(passphrase);
            core.ReadQuestions();
            DatabaseConnection connection = new DatabaseConnection();
            string publicKey = core.PublicKey;
            string dbPublicKey = connection.getPublicKey("server");
            connection.close();
            StreamWriter writer = new StreamWriter(core.ActionLogFile, true);        
            if (publicKey != dbPublicKey)
            {
                writer.Write(DateTime.Now.ToString() + Core.space);
                writer.WriteLine("Server's old public key:\n" + dbPublicKey);
                connection = new DatabaseConnection();
                connection.setPublicKey("server", "server", publicKey);
                Console.Write(DateTime.Now.ToString() + Core.space);
                Console.WriteLine("New key pair is set.");
            }
            connection.close();
            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine("Server's public key:\n" + publicKey);

            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine("Server started");
            writer.Close();
            Console.Write(DateTime.Now.ToString() + Core.space);
            Console.WriteLine("Server started.");

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
