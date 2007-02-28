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
        private const string uid = "PractiSES";
        private const string pwd = "bilkent";
        private const string dbase = "PractiSES";
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader read;
        public DatabaseConnection()
        {
            
            connectionstring = String.Format("server={0};uid={1};pwd={2};database={3}", server, uid, pwd, dbase);
            conn = new MySqlConnection(connectionstring);
            conn.Open();
        }
        public string getPublicKey(string email)
        {
            string query = "SELECT k.key from users u, keys k WHERE u.email='" + email + "';";
            
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            // Always call Read before accessing data.
            if(read.Read())
            {
                return read.GetString(1);
            }
            return "";

        }
        //public void insertEntry( 
        public void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }



    }
}
