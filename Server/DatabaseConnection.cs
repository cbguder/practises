using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
namespace PractiSES
{
    public class DatabaseConnection
    {
        private string connectionstring;
        private const string server = "pgp.sabanciuniv.edu";
        private const string uid = "practises";
        private const string pwd = "bilkent";
        private const string dbase = "practises";
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader read;
        public DatabaseConnection()
        {
            connectionstring = String.Format("server={0};uid={1};pwd={2};database={3}", server, uid, pwd, dbase);
            conn = new MySqlConnection(connectionstring);
            conn.Open();
        }
        public string getPublicKey(string email) //return public key (complete)
        {
            string query = "SELECT k.key from users u, `keys` k WHERE u.email='" + email + "';";
            
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();

            if(read.Read())
            {
                return read.GetString(0);
            }
            return "No records exist";

        }
        public void insertEntry()
        {

        }
        public bool removeEntry(string email, string userID) //remove entry (complete)
        {
            try
            {
                string query = "DELETE FROM users WHERE email='" + email + "';";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = "DELETE FROM users WHERE userID='" + userID + "';";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false; 
            }
        }
        public void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }



    }
}
