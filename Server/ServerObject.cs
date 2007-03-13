using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using PractiSES;

namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        //complete the userID-email verification query
        public string InitKeySet_AskQuestions(string userID, string email)
        {
            Core core = new Core();
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
            Core core = new Core();
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
           Core core = new Core();
           string privateKey = core.PrivateKey;
           
           AESInfo aesInfo = new AESInfo();
           string answers = Crypto.Decrypt(answersEnveloped, privateKey, ref aesInfo);

           DatabaseConnection connection = new DatabaseConnection();
           string result = connection.getAnswers(email);
           connection.close();
           if (answers == result)
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
            HashMAC mac = new HashMAC();
            string macPassword = mac.SecretKey();

            string macPassword_encrypted = Crypto.Encrypt(macPassword, aesInfo);

            return macPassword_encrypted;
        }

        private void InitKeySet_SendMail(string email, AESInfo aesInfo)
        {
            string macPassword_encrypted = InitKeySet_EncryptMACPass(email, aesInfo);
            string subject = "Welcome to PractiSES";
            string body = "Your encrypted password is " + macPassword_encrypted + 
                "Please provide your public key.";
            Email mailer = new Email(email, subject, body);    //recepient, subject, body
            mailer.Send();
            Console.WriteLine("Mail sent to user " + email); 
        }
        
        public string KeyObt(string email) //get public key of a user ( complete )
        {
            Console.WriteLine("Connected");
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
