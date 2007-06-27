using System;
using System.Security.Cryptography.X509Certificates;
using WseX509 = Microsoft.Web.Services3.Security.X509;

namespace PractiSES
{
    public static class Certificate
    {
        private static X509Store myStore;

        public static void OpenCertificate()
        {
            // Open and read the Personal certificate store for 
            // the local machine account.
            //X509SecurityToken securityToken = null;
            myStore = new X509Store(StoreName.CertificateAuthority,
                                    StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadWrite);
        }

        public static byte[] SearchCertificate(String certName)
        {
            // Search for all certificates named certName 
            // add all matching certificates 
            // to the certificate collection.
            OpenCertificate();
            X509Certificate2Collection myCerts;
            try
            {
                myCerts = myStore.Certificates.Find(X509FindType.FindBySubjectName, certName, true);
            }
            catch (NullReferenceException e)
            {
                return null;
            }
            X509Certificate2 myCert;

            // Find the first certificate in the collection 
            // that matches the supplied name, if any.
            if (myCerts.Count > 0)
            {
                myCert = myCerts[0];
                //securityToken = new X509SecurityToken(myCert);
            }
            else
            {
                return null;
            }

            // Make sure that we have a certificate 
            // that can be used for encryption.
            /*if (myCert == null ||
                !myCert..SupportsDataEncryption)
            {
                throw new ApplicationException(
                  "Service is not able to encrypt the response");
                return null;
            }*/
            /*Console.WriteLine("SerialNumberString: " + myCert.GetSerialNumberString() + "\n" +
                "PublicKeyString: " + myCert.GetPublicKeyString() + "\n" + "NameInfo: " + myCert.GetNameInfo(X509NameType.SimpleName,true) + "\n" +
                "HasPrivateKey: " + myCert.HasPrivateKey + "\nIssuer: " + myCert.Issuer + "\nNotAfter: " + myCert.NotAfter + "\nNotBefore: " + myCert.NotBefore
                + "\nPrivateKey: " + myCert.PrivateKey.ToXmlString(true) + "\nSerialNumber: " + myCert.SerialNumber + "\nSignatureAlgorithm: " + myCert.SignatureAlgorithm.Value
                + "\nSubject: " + myCert.Subject + "\nThumbprint: " + myCert.Thumbprint + "\nVersion: " + myCert.Version);*/
            Console.WriteLine("Subject: " + myCert.Subject);
            byte[] rawCertData = myCert.GetRawCertData();
            return rawCertData;
        }

        public static bool AddCertificate(byte[] data)
        {
            OpenCertificate();
            X509Certificate2 certName = new X509Certificate2(data);
            myStore.Add(certName);
            return true;
        }

        /*public static String GetPublicKey(byte[] data)
        {
            OpenCertificate();
            X509Certificate2 myCert = new X509Certificate2(data);
            String publicKey = myCert.GetPublicKeyString();
            return publicKey;
        }*/

        public static byte[] GetPublicKey(byte[] data)
        {
            OpenCertificate();
            X509Certificate2 myCert = new X509Certificate2(data);
            byte[] publicKey = myCert.GetPublicKey();
            return publicKey;
        }

        public static String GetHostName(byte[] data)
        {
            OpenCertificate();
            X509Certificate2 myCert = new X509Certificate2(data);
            String[] subjectName = myCert.SubjectName.Name.Split(',');//myCert.Subject.;
            String hostName = subjectName[0];
            hostName = hostName.Substring(3);
            return hostName;
        }
    }
}