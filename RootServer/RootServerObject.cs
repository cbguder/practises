using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public class RootServerObject : MarshalByRefObject, IRootServer
    {
        public byte[] GetCertificate(String domainName)
        {
            byte[] rawCertData;
            rawCertData = Certificate.SearchCertificate(domainName);
            //Console.WriteLine(cert.IssuerName.Name);
            //String certFields = cert.IssuerName.Name + "," + cert.PublicKey + "," + cert.SubjectName + "," + cert.SubjectName.Name + "," + cert.SerialNumber;
            return rawCertData;
        }
    }
}
