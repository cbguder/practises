using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        
        public string KeyObt(String email)
        {
            Console.WriteLine("Connected");
            DatabaseConnection connection = new DatabaseConnection();
            return connection.getPublicKey(email);
            
        }

        public bool KeyRem(string userID, string email, string signedMessage)
        {
            return true;
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
