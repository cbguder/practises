using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {

        public string InitKeySet_AskQuestions(String userID, string email)
        {
            Core core = new Core();
            string questions = core.ReadQuestionsFromSettingsFile();

            Encryption encryption = new Encryption();
            string signQuestions = encryption.SignString(questions);

            return string.Concat(questions, signQuestions);

            /*Console.WriteLine("Connected");
            DatabaseConnection connection = new DatabaseConnection();
            string result = connection.setPublicKey(email);
            connection.close();
            return result;*/
        }

        public string InitKeySet_EncryptMAC(String userID, string email)
        {
            Core core = new Core();
            string questions = core.ReadQuestionsFromSettingsFile();

            Encryption encryption = new Encryption();
            string signQuestions = encryption.SignString(questions);

            return string.Concat(questions, signQuestions);
        }
        
        public string KeyObt(String email) //get public key of a user ( complete )
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
