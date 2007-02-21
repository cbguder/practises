using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace PractiSES
{
    class Client
    {
        static string host = "10.90.10.72";

        static void Main(string[] args)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            ServerObject obj = (ServerObject)Activator.GetObject(typeof(PractiSES.ServerObject), "http://" + host + "/PractiSES");
            
            if (obj.Equals(null))
            {
                System.Console.WriteLine("Error: unable to locate server");
            }
            else
            {
                Console.WriteLine(obj.GetPublicKey("cbgder@su.sabanciuniv.edu"));
            }

            Console.ReadLine();
        }
    }
}
