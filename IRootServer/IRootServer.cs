using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public interface IRootServer
    {
        byte[] GetCertificate(String domainName);
        bool Hello();
    }
}
