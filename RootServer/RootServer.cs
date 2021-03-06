using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Collections;

namespace PractiSES
{
    class RootServer
    {
        static void Main(string[] args)
        {
            Certificate.OpenCertificate();

            Console.WriteLine("PractiSES Root Server started.");

            HttpServerChannel channel = new HttpServerChannel(88);
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PractiSES.RootServerObject),
                "PractiSES_Root",
                WellKnownObjectMode.SingleCall);
            System.Console.ReadLine();
        }
    }
}
