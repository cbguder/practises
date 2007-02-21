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
            ChannelServices.RegisterChannel(chan);

            SampleObject obj = (SampleObject)Activator.GetObject(typeof(CodeGuru.Remoting.SampleObject), "tcp://localhost:8080/HelloWorld");

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
