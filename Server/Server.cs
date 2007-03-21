using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;


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
            DatabaseConnection connection = new DatabaseConnection();
            string publicKey = core.PublicKey;
            string dbPublicKey = connection.getPublicKey("server");
            connection.close();
            if (publicKey != dbPublicKey)
            {
                connection = new DatabaseConnection();
                connection.setPublicKey("server", "server", publicKey);
                Console.WriteLine("Old public key:\n" + dbPublicKey);
                Console.WriteLine("New key pair is set.");
            }   
            connection.close();
            Console.WriteLine("Server's public key:\n" + publicKey);
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
