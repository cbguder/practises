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

            HttpServerChannel channel = new HttpServerChannel(88);
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PractiSES.RootServerObject),
                "PractiSES_Root",
                WellKnownObjectMode.SingleCall);
            System.Console.ReadLine();



            /*Console.WriteLine("Server Process Started.");
            //BinaryFormatter
            BinaryServerFormatterSinkProvider bp = new BinaryServerFormatterSinkProvider();
            //bp.TypeFilterLevel = TypeFilterLevel.Full;
            //ClientIPServerSinkProvider implemented in "ServerSink.cs".
            ClientIPServerSinkProvider csp = new ClientIPServerSinkProvider();
            //Chain the SinkProviders.
            csp.Next = bp;
            Hashtable ht = new Hashtable();
            ht.Add("port",9090);
            //TcpChannel tcpc = new TcpChannel(ht,null,csp);
            HttpServerChannel channel = new HttpServerChannel("PractiSES_Root", 88, csp);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PractiSES.RootServerObject),
                "PractiSES_Root",
                WellKnownObjectMode.SingleCall);
            Console.ReadLine(); */
        }
    }
}
