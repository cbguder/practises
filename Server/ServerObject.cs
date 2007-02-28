using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        
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
