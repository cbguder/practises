using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using PractiSES;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace PractiSES
{
    public class ServerObject : MarshalByRefObject, IServer
    {
        private const String rootHost = "10.80.10.178";
        private IRootServer rootServer;
       

        public String InitKeySet_AskQuestions(String userID, String email)
        {
            Core core = new Core(Server.passphrase);
            StreamWriter writer = new StreamWriter(core.ActionLogFile, true);
            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine("-------------------------");
            writer.WriteLine(email + ": InitKeySet_AskQuestions");
            writer.Close();
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": InitKeySet_AskQuestions");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return null;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return null;
            }
            String questions = core.ReadQuestions();
            String signQuestions = Crypto.Sign(questions, core.PrivateKey);
            return signQuestions;
        }

        public bool InitKeySet_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": InitKeySet_EnvelopeAnswers");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            //connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return false;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return false;
            }
            Core core = new Core(Server.passphrase);
            String privateKey = core.PrivateKey;

            Rijndael aes = Rijndael.Create();
            AESInfo aesInfo = Crypto.Destruct(answersEnveloped, privateKey);
            String answers = Encoding.UTF8.GetString(Crypto.AESDecrypt(aesInfo.message, aes.CreateDecryptor(aesInfo.key, aesInfo.IV)));

          //  connection = new DatabaseConnection();
            String dbAnswers = connection.getAnswers(email);
            connection.close();
            if (answers == dbAnswers)
            {
                InitKeySet_SendMail(email, aesInfo);
                return true;
            }
            else
            {
               //protocol stops and socket is closed.
                InitKeySet_ErrorMail(email);
                Console.WriteLine("Error - " + email + ": Answers are not correct!");
                return false;
            }
        }

        private String InitKeySet_EncryptMACPass(String email, AESInfo aesInfo)
        {
            HMAC hmac = HMACSHA1.Create();

            Rijndael aes = Rijndael.Create();

            DatabaseConnection connection = new DatabaseConnection();
            connection.setMACPass(email, Convert.ToBase64String(hmac.Key));
            connection.close();

            String result = Util.Wrap(Convert.ToBase64String(Crypto.AESEncrypt(hmac.Key, aes.CreateEncryptor(aesInfo.key, aesInfo.IV))), 64);
            return result;
        }

        private void InitKeySet_SendMail(String email, AESInfo aesInfo)
        {
            String macPassword_encrypted = InitKeySet_EncryptMACPass(email, aesInfo);
            String subject = "Welcome to PractiSES";
            StringBuilder body = new StringBuilder("Please double click the message to open this mail message in new window.\n Then follow Tools -> PractiSES -> Finalize Initialization links to finish initialization.");
            body.AppendLine();
            body.AppendLine(Crypto.BeginMessage);
            body.AppendLine();
            body.AppendLine(macPassword_encrypted);
            body.AppendLine(Crypto.EndMessage);
            Email mailer = new Email(email, subject, body.ToString());    //recepient, subject, body
            if(mailer.Send())
            {
                Console.WriteLine(email + ": Mail sent."); 
            }
        }

        private void InitKeySet_ErrorMail(String email)
        {
            String subject = "PractiSES";
            String body = "Your answers are not correct.";
            Email mailer = new Email(email, subject, body);    //recepient, subject, body
            mailer.Send();
            Console.WriteLine(email + ": Warning mail sent.");
        }

        public bool InitKeySet_SendPublicKey(String userID, String email, String publicKey, String macValue)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": InitKeySet_SendPublicKey");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
         //   connection.close();
            if (userID == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                return false;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine("Error - " + email + ": User id does not exist!");
                return false;
            }
          //  connection = new DatabaseConnection();
            String dbMACPass = connection.getMACPass(email);
         //   connection.close();

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(dbMACPass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(publicKey));
            //Hash hash = new Hash(dbMACPass);
            //if (hash.ValidateMAC(publicKey, macValue))
            if(Util.Compare(hash, Convert.FromBase64String(macValue)))
            {
             //   connection = new DatabaseConnection();
                connection.setPublicKey(userID, email, publicKey);
             //   connection.close();
            //    connection = new DatabaseConnection();
                connection.removeMACPass(email);
                connection.close();
                Console.WriteLine(email +": Public key is set.");
                return true;
            }
            Console.WriteLine("Error - " + email + ": MAC value is tampered, public key is not set.");
            return false;
        }
        
        public String KeyObt(String email) //get public key of a user ( complete )
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyObt");
            DatabaseConnection connection = new DatabaseConnection();
            String publicKey = connection.getPublicKey(email);
            connection.close();
            if (publicKey == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
            }
            return publicKey;
        }

        //date de eklenecek, settings e domain name eklenecek
        public String KeyObt(String email, DateTime date) //get public key of a user ( complete )
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyObt");
            int index = email.IndexOf('@');
            String domainName = email.Substring(index, email.Length - index); 
            String publicKey = null;
            DatabaseConnection connection = new DatabaseConnection();
            if ("@su.sabanciuniv.edu" == domainName)
            {
                
                publicKey = connection.getPublicKey(email);
               // connection.close();
                if (publicKey == null)
                {
                    Console.WriteLine("Error - " + email + ": Email does not exist!");
                }
            }
            else
            {
                byte[] rawCertData = Certificate.SearchCertificate(domainName);
                if (rawCertData == null)
                {         
                    if (Connect(rootHost))
                    {
                        if (GetCertificate(domainName))
                        {
                           // DatabaseConnection connection = new DatabaseConnection();
                            publicKey = connection.getPublicKey(email, date);
                            connection.close();
                            if (publicKey == null)
                            {
                                Console.WriteLine("Error - " + email + ": Email does not exist!");
                            }
                        }
                    }
                }
            }
            return publicKey;
        }

        public bool KeyRem(String userID, String email, String signedMessage)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("Connected");
            DatabaseConnection connection = new DatabaseConnection();
            bool result = connection.removeEntry(email, userID);
            connection.close();
            return result;
        }

        public bool KeyUpdate(String userID, String email, String signedMessage)
        {
            Console.WriteLine("-------------------------");
            return true;
        }


        /************************************************************************/
        public String USKeyRem_AskQuestions(String userID, String email)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyRem_AskQuestions");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return null;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return null;
            }
            Core core = new Core(Server.passphrase);
            String questions = core.ReadQuestions();
            String signQuestions = Crypto.Sign(questions, core.PrivateKey);
            return signQuestions;
        }

        public void USKeyRem_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyRem_EnvelopeAnswers");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
           // connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return;
            }
            Core core = new Core(Server.passphrase);
            String privateKey = core.PrivateKey;

            Rijndael aes = Rijndael.Create();
            AESInfo aesInfo = Crypto.Destruct(answersEnveloped, privateKey);
            String answers = Encoding.UTF8.GetString(Crypto.AESDecrypt(aesInfo.message, aes.CreateDecryptor(aesInfo.key, aesInfo.IV)));

           // connection = new DatabaseConnection();
            String dbAnswers = connection.getAnswers(email);
            connection.close();
            if (answers == dbAnswers)
            {
                USKeyRem_SendMail(email, aesInfo);
            }
            else
            {
                //protocol stops and socket is closed.
                InitKeySet_ErrorMail(email);
                Console.WriteLine("Error - " + email + ": Answers are not correct!");
            }
        }

        private void USKeyRem_SendMail(String email, AESInfo aesInfo)
        {
            String macPassword_encrypted = InitKeySet_EncryptMACPass(email, aesInfo);
            String subject = "Your PractiSES Key Removal Request";
            StringBuilder body = new StringBuilder("Please double click the message to open this mail message in new window.\n Then follow Tools -> PractiSES -> Finalize Key Removal links to finish removal of your key.");
            body.AppendLine();
            body.AppendLine(Crypto.BeginMessage);
            body.AppendLine();
            body.AppendLine(macPassword_encrypted);
            body.AppendLine(Crypto.EndMessage);
            Email mailer = new Email(email, subject, body.ToString());    //recepient, subject, body
            mailer.Send();
            Console.WriteLine(email + ": Mail sent.");
        }

        public bool USKeyRem_SendRemoveRequest(String userID, String email, String macValue)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyRem_SendPublicKey");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
           // connection.close();
            if (userID == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                return false;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine("Error - " + email + ": User id does not exist!");
                return false;
            }
           // connection = new DatabaseConnection();
            String dbMACPass = connection.getMACPass(email);
          //  connection.close();

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(dbMACPass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("I want to remove my current public key"));

            if (Convert.ToBase64String(hash) == macValue)
            {
             //   connection = new DatabaseConnection();
                connection.removePublicKey(userID, email);
                connection.close();
                Console.WriteLine(email + ": Public key is removed.");
                return true;
            }

            Console.WriteLine("Error - " + email + ": MAC value is tampered, public key is not set.");
            return false;
        }
        /***************************************************************************/

        public String USKeyUpdate_AskQuestions(String userID, String email)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_AskQuestions");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return null;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return null;
            }
            Core core = new Core(Server.passphrase);
            String questions = core.ReadQuestions();
            String signQuestions = Crypto.Sign(questions, core.PrivateKey);
            return signQuestions;
        }

        public void USKeyUpdate_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_EnvelopeAnswers");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
          //  connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                return;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                return;
            }
            Core core = new Core(Server.passphrase);
            String privateKey = core.PrivateKey;

            Rijndael aes = Rijndael.Create();
            AESInfo aesInfo = Crypto.Destruct(answersEnveloped, privateKey);
            String answers = Encoding.UTF8.GetString(Crypto.AESDecrypt(aesInfo.message, aes.CreateDecryptor(aesInfo.key, aesInfo.IV)));

          //  connection = new DatabaseConnection();
            String dbAnswers = connection.getAnswers(email);
            connection.close();
            if (answers == dbAnswers)
            {
                USKeyUpdate_SendMail(email, aesInfo);
            }
            else
            {
                //protocol stops and socket is closed.
                USKeyUpdate_ErrorMail(email);
                Console.WriteLine("Error - " + email + ": Answers are not correct!");
            }
        }

        private String USKeyUpdate_EncryptMACPass(String email, AESInfo aesInfo)
        {
            HMAC hmac = HMACSHA1.Create();

            Rijndael aes = Rijndael.Create();

            DatabaseConnection connection = new DatabaseConnection();
            connection.setMACPass(email, Convert.ToBase64String(hmac.Key));
            connection.close();

            return Convert.ToBase64String(Crypto.AESEncrypt(hmac.Key, aes.CreateEncryptor(aesInfo.key, aesInfo.IV)));
        }

        private void USKeyUpdate_SendMail(String email, AESInfo aesInfo)
        {
            String macPassword_encrypted = InitKeySet_EncryptMACPass(email, aesInfo);
            String subject = "Welcome to PractiSES";
            StringBuilder body = new StringBuilder("Please double click the message to open this mail message in new window.\n Then follow Tools -> PractiSES -> Finalize Update links to finish update operation.");
            body.AppendLine();
            body.AppendLine(Crypto.BeginMessage);
            body.AppendLine();
            body.AppendLine(macPassword_encrypted);
            body.AppendLine(Crypto.EndMessage);
            Email mailer = new Email(email, subject, body.ToString());    //recepient, subject, body
            mailer.Send();
            Console.WriteLine(email + ": Mail sent.");
        }

        private void USKeyUpdate_ErrorMail(String email)
        {
            String subject = "PractiSES";
            String body = "Your answers are not correct.";
            Email mailer = new Email(email, subject, body);    //recepient, subject, body
            mailer.Send();
            Console.WriteLine(email + ": Warning mail sent.");
        }

        public bool USKeyUpdate_SendPublicKey(String userID, String email, String publicKey, String macValue)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_SendPublicKey");
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
           // connection.close();
            if (userID == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                return false;
            }
            if (userID != dbUserid)
            {
                Console.WriteLine("Error - " + email + ": User id does not exist!");
                return false;
            }
           // connection = new DatabaseConnection();
            String dbMACPass = connection.getMACPass(email);
           // connection.close();

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(dbMACPass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(publicKey));
       
            if (Util.Compare(hash, Convert.FromBase64String(macValue)))
            {
               // connection = new DatabaseConnection();
                connection.setPublicKey(userID, email, publicKey);
                connection.close();
                Console.WriteLine(email + ": Public key is set.");
                return true;
            }
            Console.WriteLine("Error - " + email + ": MAC value is tampered, public key is not set.");
            return false;
        }
        
        /***************************************************************************/

        /*private X509Certificate2 GetCertificate(String domainName)
        {
            return rootServer.GetCertificate(domainName);
        }*/

        private bool Connect(String host)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.WriteLine("Connecting to PractiSES root server ({0})...", host);
            rootServer = (IRootServer)Activator.GetObject(typeof(IRootServer), "http://" + host + "/PractiSES_Root");
            Console.WriteLine("Connected.");

            return true;
        }

        private bool GetCertificate(String domainName)
        {
            byte[] rawCertData = rootServer.GetCertificate(domainName);
            if (rawCertData != null)
            {
                Certificate.AddCertificate(rawCertData);
                Console.WriteLine("Certificate has been downloaded successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Certificate is not found.");
                return false;
            }
        }
    }
}
