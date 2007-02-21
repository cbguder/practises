using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace PractiSES
{

    class Server
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel,false);

            // Register as an available service with the name HelloWorld
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PractiSES.ServerObject),
                "GetPublicKey",
                WellKnownObjectMode.SingleCall);
            System.Console.ReadLine();

        }
    }
}
