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
            try
            {
                connectionstring = String.Format("server={0};uid={1};pwd={2};database={3}", server, uid, pwd, dbase);
                conn = new MySqlConnection(connectionstring);
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                throw e;
            }
        }

        public string getUserID(string email) //return user id  (complete)
        {
            string query = "SELECT u.userid from users u WHERE u.email='" + email + "';";
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                return read.GetString(0);
            }
            return null;

        }

        public bool setPublicKey(string userID, string email, string key) 
        {
            try
            {
                string query = String.Format("INSERT INTO `keys` (`userID`, `start`, `end`, `key`, `deleted`) VALUES ('{0}', NOW(), 0, '{1}', 0);", userID, key);
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false; 
            }
        }

        public bool updatePublicKey(string userID, string email, string key)
        {
            try
            {
                string query = String.Format("UPDATE `keys` SET `key` = '{0}' WHERE `userID` ='{1}';",key,userID);
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public bool removePublicKey(string userID, string email)
        {
            try
            {
                string query = String.Format("UPDATE `keys` SET `key` = '' WHERE `userID` ='{0}';", userID);
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }
        public string getPublicKey(string email) //return public key (complete)
        {
            string query = "SELECT k.key from users u, `keys` k WHERE u.email='" + email + "' AND k.userid=u.userid ORDER BY k.`start` DESC LIMIT 1;";
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                return read.GetString(0);
            }
            return null;
        }

        public string getPublicKey(string email, DateTime date)
        {
            DateTime unixStart = new DateTime(1970, 1, 1);
            TimeSpan timestamp = date - unixStart;
            string query = string.Format("SELECT k.key from users u, `keys` k WHERE u.email='{0}' AND k.userid=u.userid AND k.`start`<'{1}' AND (k.`end`=0 OR k.`end`>'{2}');", email, timestamp, timestamp);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                return read.GetString(0);
            }
            return null;
        }

        public string getAnswers(string email)
        {
            string query = "SELECT u.semisecret1 from users u WHERE u.email='" + email + "';";   
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if(read.Read())
            {
                return read.GetString(0);
            }
            return email + ": No records exist";
        }

        public bool setMACPass(string email, string key)
        {
            try
            {
                string query = "UPDATE users u SET u.macpass='" + key + "' WHERE u.email='" + email + "';";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public string getMACPass(string email)
        {
            string query = "SELECT u.macpass from users u WHERE u.email='" + email + "';";
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();         
            if (read.Read())
            {
                return read.GetString(0);
            }
            return "No records exist";
        }

        public string removeMACPass(string email)
        {
            string query = string.Format("UPDATE users u SET u.macpass=NULL WHERE u.email='{0}';", email);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
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
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
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
