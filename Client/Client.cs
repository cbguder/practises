using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace PractiSES
{
    class Client
    {
        static void Main(string[] args)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, true);

            ServerObject obj = (ServerObject)Activator.GetObject(typeof(PractiSES.ServerObject), "http://10.90.10.62/HelloWorld");

            if (obj.Equals(null))
            {
                System.Console.WriteLine("Error: unable to locate server");
            }
            else
            {
                Console.WriteLine(obj.HelloWorld());
            }
        }
    }
}
