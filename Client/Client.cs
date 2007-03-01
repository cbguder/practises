/*
 * $Id$
 */

using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Security.Cryptography;

namespace PractiSES
{
    class Client
    {
        private const String host = "practises.no-ip.org";

        static void Main(string[] args)
        {
            Core core = new Core();

            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.WriteLine("Connecting to PractiSES server ({0})...", host);

            ServerObject server = (ServerObject)Activator.GetObject(typeof(ServerObject), "http://" + host + "/PractiSES");

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
