using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PractiSES
{
    public class Certificate
    {
        //char[] chr = new char[8];
        //char* chr2 = chr;
        //SecureString secureStr = new SecureString(chr, 8);
        //X509CertificateStore certificateStore = new X509CertificateCollection(
        //X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;
        public Certificate(String fileName)
        {
            X509Certificate certificate = new X509Certificate(fileName);
            if (certificate.Issuer == "PractiSES Root CA")
            {
                //certificate.GetExpirationDateString
            }
            
        }
        
        
    }
}
