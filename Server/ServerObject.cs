using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        public string HelloWorld()
        {
            return "Hello World!";
        }
    }
}
