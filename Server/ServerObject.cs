using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using PractiSES;
using System.Security.Cryptography;

namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        //complete the userID-email verification query
        public string InitKeySet_AskQuestions(string userID, string email)
        {
            Console.WriteLine(email + ": InitKeySet_AskQuestions");
            DatabaseConnection connection = new DatabaseConnection();
            string result = connection.getUserID(email);
            connection.close();
            if (result != userID)
            {
                Console.WriteLine("Incorrect user id or e-mail address");
                return "Error: Incorrect user id or e-mail address";
            }

            Core core = new Core(Server.passphrase);
            string questions = core.ReadQuestions();

            string signQuestions = Crypto.Sign(questions, core.PrivateKey);

            //questions = string.Concat(questions, signQuestions);
            return signQuestions;

            /*Console.WriteLine("Connected");
            DatabaseConnection connection = new DatabaseConnection();
            string result = connection.setPublicKey(email);
            connection.close();
            return result;*/
        }

        /*public void InitKeySet_EnvelopeAnswers(string userID, string email, string SK_encrypted, string Answers_encrypted)
        {       
            Core core = new Core(Server.passphrase);
            string privateKey = core.PrivateKey;

            string SK = Crypto.Decrypt(SK_encrypted, privateKey);
            string answers = Crypto.Decrypt(Answers_encrypted, SK);

            DatabaseConnection connection = new DatabaseConnection();
            string result = connection.getAnswers(email);
            connection.close();
            if (answers == result)
            {
                InitKeySet_SendMail(email, SK);
            }
            else
            {
                //protocol stops and socket is closed.
            }
        }*/

        public void InitKeySet_EnvelopeAnswers(string userID, string email, string answersEnveloped)
        {
            Console.WriteLine(email + ": InitKeySet_EnvelopeAnswers");
            DatabaseConnection connection = new DatabaseConnection();
            string dbUserid = connection.getUserID(email);
            connection.close();
            if (userID != dbUserid)
            {
                Console.WriteLine("Incorrect user id or e-mail address");
                return;
            }

            Core core = new Core(Server.passphrase);
            string privateKey = core.PrivateKey;

            Rijndael aes = Rijndael.Create();

            AESInfo aesInfo = Crypto.Destruct(answersEnveloped, privateKey);
            String answers = Encoding.UTF8.GetString(Crypto.AESDecrypt(aesInfo.message, aes.CreateDecryptor(aesInfo.key, aesInfo.IV)));

            connection = new DatabaseConnection();
            string dbAnswers = connection.getAnswers(email);
            connection.close();
            if (answers == dbAnswers)
            {
               InitKeySet_SendMail(email, aesInfo);
            }
            else
            {
               //protocol stops and socket is closed.
            }
        }

        private string InitKeySet_EncryptMACPass(string email, AESInfo aesInfo)
        {
            HMAC hmac = HMACSHA1.Create();

            Rijndael aes = Rijndael.Create();

            DatabaseConnection connection = new DatabaseConnection();
            connection.setMACPass(email, Convert.ToBase64String(hmac.Key));
            connection.close();

            return Convert.ToBase64String(Crypto.AESEncrypt(hmac.Key, aes.CreateEncryptor(aesInfo.key, aesInfo.IV)));
        }

        private void InitKeySet_SendMail(string email, AESInfo aesInfo)
        {
            string macPassword_encrypted = InitKeySet_EncryptMACPass(email, aesInfo);
            string subject = "Welcome to PractiSES";
            string body = macPassword_encrypted;
            Email mailer = new Email(email, subject, body);    //recepient, subject, body
            mailer.Send();
            Console.WriteLine("Mail sent to user " + email); 
        }

        public bool InitKeySet_SendPublicKey(string userID, string email, string publicKey, string macValue)
        {
            Console.WriteLine(email + ": InitKeySet_SendPublicKey");
            DatabaseConnection connection = new DatabaseConnection();
            string dbUserid = connection.getUserID(email);
            connection.close();
            if (userID != dbUserid)
            {
                Console.WriteLine("Incorrect user id or e-mail address");
                return false;
            }

            connection = new DatabaseConnection();
            string dbMACPass = connection.getMACPass(email);
            connection.close();

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(dbMACPass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(publicKey));

            //Hash hash = new Hash(dbMACPass);
            //if (hash.ValidateMAC(publicKey, macValue))
            if(Util.Compare(hash, Convert.FromBase64String(macValue)))
            {
                connection.setPublicKey(email, publicKey);
                Console.WriteLine(email +": Public key is set.");
                return true;
            }
            Console.WriteLine(email + ": Error - Public key could not be set.");
            return false;
        }
        
        public string KeyObt(string email) //get public key of a user ( complete )
        {
            Console.WriteLine(email + ": KeyObt");
            DatabaseConnection connection = new DatabaseConnection();
            string result = connection.getPublicKey(email);
            connection.close();
            return result;
        }

        public bool KeyRem(string userID, string email, string signedMessage)
        {
            Console.WriteLine("Connected");
            DatabaseConnection connection = new DatabaseConnection();
            bool result = connection.removeEntry(email, userID);
            connection.close();
            return result;
        }

        public bool KeyUpdate(string userID, string email, string signedMessage)
        {
            return true;
        }

        public void USKeyRem(string userID, string email)
        {
        }

        public void USKeyUpdate(string userID, string email, string newKey)
        {
        }

    }
}
