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
