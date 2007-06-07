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
        private const String rootHost = "practises3.no-ip.org";
        private const String beginProtocol = "-------------------------";
        private IRootServer rootServer;
       
        private void ActionLog_Write(String logMessage)
        {
            Core core = new Core(Server.passphrase);
            StreamWriter writer = new StreamWriter(core.ActionLogFile, true);
            writer.Write(DateTime.Now.ToString() + Core.space);
            writer.WriteLine(beginProtocol);
            writer.WriteLine(logMessage);
            writer.Close();
        }

        private String AskQuestions(String userID, String email)
        {

            Core core = new Core(Server.passphrase);
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            connection.close();

            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                throw new Exception("Invalid user");
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                throw new Exception("Invalid user");
            }
            String questions = core.ReadSettingsFile();
            Message result = new Message(questions);
            result.Sign(core.PrivateKey);
            return result.ToString();
        }

        private bool EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            //connection.close();
            if (userID == null)
            {
                Console.WriteLine(email + ": Email does not exist!");
                throw new Exception("Invalid user");
            }
            if (userID != dbUserid)
            {
                Console.WriteLine(email + ": User id does not exist!");
                throw new Exception("Invalid user");
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
                SendMail(email, aesInfo);
                return true;
            }
            else
            {
                //protocol stops and socket is closed.
                ErrorMail(email);
                Console.WriteLine("Error - " + email + ": Answers are not correct!");
                throw new Exception("Answers are not correct");
            }
        }

        private String EncryptMACPass(String email, AESInfo aesInfo)
        {
            HMAC hmac = HMACSHA1.Create();

            Rijndael aes = Rijndael.Create();

            DatabaseConnection connection = new DatabaseConnection();
            connection.setMACPass(email, Convert.ToBase64String(hmac.Key));
            connection.close();

            String result = Util.Wrap(Convert.ToBase64String(Crypto.AESEncrypt(hmac.Key, aes.CreateEncryptor(aesInfo.key, aesInfo.IV))), 64);
            return result;
        }

        private void SendMail(String email, AESInfo aesInfo)
        {
            String macPassword_encrypted = EncryptMACPass(email, aesInfo);
            String subject = "PractiSES notification";
            StringBuilder body = new StringBuilder("Please double click the message to open this mail message in new window.\n Then follow Tools -> PractiSES -> Finalize Initialization links to finish initialization.");
            body.AppendLine();
            body.AppendLine(Crypto.BeginMessage);
            body.AppendLine();
            body.AppendLine(macPassword_encrypted);
            body.AppendLine(Crypto.EndMessage);
            Email mailer = new Email(email, subject, body.ToString());    //recepient, subject, body
            if (mailer.Send())
            {
                Console.WriteLine(email + ": Mail sent.");
            }
        }

        private void ErrorMail(String email)
        {
            String subject = "PractiSES";
            String body = "Your answers are not correct.";
            Email mailer = new Email(email, subject, body);    //recepient, subject, body
            mailer.Send();
            Console.WriteLine(email + ": Warning mail sent.");
        }

        private bool SendQuery(String userID, String email, String message, String macValue)
        {
            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
            //   connection.close();
            if (userID == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                throw new Exception("Invalid user");
            }
            if (userID != dbUserid)
            {
                Console.WriteLine("Error - " + email + ": User id does not exist!");
                throw new Exception("Invalid user");
            }
            //  connection = new DatabaseConnection();
            String dbMACPass = connection.getMACPass(email);
            //   connection.close();

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(dbMACPass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            //Hash hash = new Hash(dbMACPass);
            //if (hash.ValidateMAC(publicKey, macValue))
            if (Util.Compare(hash, Convert.FromBase64String(macValue)))
            {
                connection.removeMACPass(email);
                connection.close();
            
                return true;
            }
            connection.close();
            Console.WriteLine("Error - " + email + ": MAC value is tampered, public key is not set.");
            throw new Exception("MAC value is tampered, public key is not set");
        }

        public String InitKeySet_AskQuestions(String userID, String email)
        {
            ActionLog_Write(email + ": InitKeySet_AskQuestions");

            Console.WriteLine(beginProtocol);
            Console.WriteLine(email + ": InitKeySet_AskQuestions");

            return AskQuestions(userID, email);
        }

        public bool InitKeySet_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            ActionLog_Write(email + ": InitKeySet_EnvelopeAnswers");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": InitKeySet_EnvelopeAnswers");

            return EnvelopeAnswers(userID, email, answersEnveloped);
        }

        public bool InitKeySet_SendPublicKey(String userID, String email, String publicKey, String macValue)
        {
            ActionLog_Write(email + ": InitKeySet_SendPublicKey");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": InitKeySet_SendPublicKey");

            if (SendQuery(userID, email, publicKey, macValue))
            {
                DatabaseConnection connection = new DatabaseConnection();
                connection.setPublicKey(userID, email, publicKey);
                connection.close();
                Console.WriteLine(email + ": Public key is set.");
                return true;
            }
            return false;
        }
        
        public String KeyObt(String email) //get public key of a user ( complete )
        {
            /*Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyObt");
            DatabaseConnection connection = new DatabaseConnection();
            String publicKey = connection.getPublicKey(email);
            connection.close();
            if (publicKey == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
            }
            Core core = new Core(Server.passphrase);
            Message message = new Message(publicKey);
            message.AddComment("Email", email);
            message.Sign(core.PrivateKey);
            String result = message.ToString();
            return result;*/
            return KeyObt(email, DateTime.Now);
        }

        //date de eklenecek, settings e domain name eklenecek
        public String KeyObt(String email, DateTime date) //get public key of a user ( complete )
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyObt");
            int index = email.IndexOf('@');
            String domainName = email.Substring(index, email.Length - index); 
            String publicKey = null;
            Core core = new Core(Server.passphrase);
            if (core.GetDomainName() == domainName)
            {
                DatabaseConnection connection = new DatabaseConnection();
                publicKey = connection.getPublicKey(email, date);
                connection.close();
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
                            rawCertData = Certificate.SearchCertificate(domainName);
                            String foreignServerPublicKey = Certificate.GetPublicKey(rawCertData);
                            String foreignServerHost = Certificate.GetHostName(rawCertData);

                            HttpClientChannel chan = new HttpClientChannel();
                            ChannelServices.RegisterChannel(chan, false);
                            Console.WriteLine("Connecting to foreign PractiSES server ({0})...", foreignServerHost);
                            IServer foreignServer = (IServer)Activator.GetObject(typeof(IServer), "http://" + foreignServerHost + "/PractiSES");
                            String signedPublicKey = foreignServer.KeyObt(email, date);
                            if (signedPublicKey != null)
                            {
                                Message foreignmessage = new Message(signedPublicKey);
                                if (foreignmessage.Verify(foreignServerPublicKey))
                                {
                                    publicKey = foreignmessage.getCleartext();
                                }
                            }                    
                        }
                    }
                }
            }
            if (publicKey == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                throw new Exception("Invalid user");
            }
            Message message = new Message(publicKey);
            message.AddComment("Email",email);
            message.Sign(core.PrivateKey);
            String result = message.ToString();
            return result;
        }

        public bool KeyRem(String userID, String email, Message signedMessage)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyRem");
            DatabaseConnection connection = new DatabaseConnection();
            String publicKey = connection.getPublicKey(email);
            if (signedMessage.Verify(publicKey))
            {
                if (DateTime.Compare(signedMessage.Time, DateTime.Now.AddHours(-1)) >= 0)
                {
                    bool result = connection.removePublicKey(userID, email);
                    connection.close();
                    return result;
                }
            }
            connection.close();
            throw new Exception("Incorrect message");
            
        }

        public bool KeyUpdate(String userID, String email, Message signedMessage)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": KeyUpdate");
            DatabaseConnection connection = new DatabaseConnection();
            String publicKey = connection.getPublicKey(email);
            if (signedMessage.Verify(publicKey))
            {
                if (DateTime.Compare(signedMessage.Time, DateTime.Now.AddHours(-1)) >= 0)
                {
                    bool result = connection.updatePublicKey(userID, email, signedMessage.getCleartext());
                    connection.close();
                    return result;
                }
            }

            connection.close();
            throw new Exception("Incorrect message");
        }

        public String USKeyRem_AskQuestions(String userID, String email)
        {
            ActionLog_Write(email + ": USKeyRem_AskQuestions");

            Console.WriteLine(beginProtocol);
            Console.WriteLine(email + ": USKeyRem_AskQuestions");

            return AskQuestions(userID, email);
        }

        public bool USKeyRem_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            ActionLog_Write(email + ": USKeyRem_EnvelopeAnswers");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyRem_EnvelopeAnswers");

            return EnvelopeAnswers(userID, email, answersEnveloped);
        }

        public bool USKeyRem_SendRemoveRequest(String userID, String email, String macValue)
        {
            ActionLog_Write(email + ": USKeyRem_SendPublicKey");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyRem_SendPublicKey");

            //SendQuery(userID,email,public


            DatabaseConnection connection = new DatabaseConnection();
            String dbUserid = connection.getUserID(email);
           // connection.close();
            if (userID == null)
            {
                Console.WriteLine("Error - " + email + ": Email does not exist!");
                throw new Exception("Invalid user");
            }
            if (userID != dbUserid)
            {
                Console.WriteLine("Error - " + email + ": User id does not exist!");
                throw new Exception("Invalid user");
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
                connection.removeMACPass(email);
                connection.close();
                Console.WriteLine(email + ": Public key is removed.");
                return true;
            }

            Console.WriteLine("Error - " + email + ": MAC value is tampered, public key is not set.");
            throw new Exception("MAC value is tampered, public key is not set.");
        }

        public String USKeyUpdate_AskQuestions(String userID, String email)
        {
            ActionLog_Write(email + ": USKeyUpdate_AskQuestions");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_AskQuestions");

            return AskQuestions(userID, email);
        }

        public bool USKeyUpdate_EnvelopeAnswers(String userID, String email, String answersEnveloped)
        {
            ActionLog_Write(email + ": USKeyUpdate_EnvelopeAnswers");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_EnvelopeAnswers");

            return EnvelopeAnswers(userID, email, answersEnveloped);
        }

        public bool USKeyUpdate_SendPublicKey(String userID, String email, String publicKey, String macValue)
        {
            ActionLog_Write(email + ": USKeyUpdate_SendPublicKey");

            Console.WriteLine("-------------------------");
            Console.WriteLine(email + ": USKeyUpdate_SendPublicKey");

            if (SendQuery(userID, email, publicKey, macValue))
            {
                DatabaseConnection connection = new DatabaseConnection();
                connection.updatePublicKey(userID, email, publicKey);
                connection.close();
                Console.WriteLine(email + ": Public key is updated.");
                return true;
            }
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
