using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PractiSES
{
    class Client
    {
        static void Main(string[] args)
        {
            TcpChannel chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);

            ServerObject obj = (ServerObject)Activator.GetObject(typeof(PractiSES.ServerObject), "tcp://10.90.10.72:8080/HelloWorld");
            
            if (obj.Equals(null))
            {
                System.Console.WriteLine("Error: unable to locate server");
            }
            else
            {
                Console.WriteLine(obj.HelloWorld());
            }

            Console.ReadLine();
        }
    }
}
