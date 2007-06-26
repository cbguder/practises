using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
namespace PractiSES
{
    public class DatabaseConnection
    {
        private string connectionstring;
        //private const string server = "pgp.sabanciuniv.edu";
        //private const string uid = "practises";
        //private const string pwd = "bilkent";
        //private const string dbase = "practises";
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader read;

        public DatabaseConnection()
        {
            try
            {
                Core core = new Core(Server.passphrase, false);

                connectionstring = String.Format("server={0};uid={1};pwd={2};database={3}", core.GetXmlNodeInnerText("server"), core.GetXmlNodeInnerText("uid"), core.GetXmlNodeInnerText("pwd"), core.GetXmlNodeInnerText("dbase"));
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
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
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
                removePublicKey(userID, email);
                setPublicKey(userID, email, key);
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
                string query = String.Format("UPDATE `keys` SET `deleted` = 1, `end` = NOW() WHERE `userID` ='{0}' AND `deleted` = 0;", userID);
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
            string query = String.Format("SELECT k.key FROM users u, `keys` k WHERE u.email='{0}' AND k.deleted=0 AND k.userid=u.userid ORDER BY k.`start` DESC LIMIT 1;", email);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
            return null;
        }

        public string getPublicKey(string email, DateTime date)
        {
            DateTime unixStart = new DateTime(1970, 1, 1);
            TimeSpan timestamp = date - unixStart;
            string query = string.Format("SELECT k.key FROM users u, `keys` k WHERE u.email='{0}' AND k.deleted=0 AND k.userid=u.userid AND k.`start`<{1} AND (k.`end`=0 OR k.`end`>{2}) ORDER BY k.`start` DESC LIMIT 1;", email, timestamp.Ticks, timestamp.Ticks);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
            return null;
        }

        public string getAnswers(string email)
        {
            string query = "SELECT u.semisecret1 from users u WHERE u.email='" + email + "';";   
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
            return email + ": No records exist";
        }

        public bool setMACPass(string email, string key)
        {
            try
            {
                string query = string.Format("UPDATE users u SET u.macpass='{0}' WHERE u.email='{1}';", key, email);
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
            string query = string.Format("SELECT u.macpass from users u WHERE u.email='{0}';", email);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
            return "No records exist";
        }

        public string removeMACPass(string email)
        {
            string query = string.Format("UPDATE users u SET u.macpass=NULL WHERE u.email='{0}';", email);
            cmd = new MySqlCommand(query, conn);
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                string result = read.GetString(0);
                read.Close();
                return result;
            }
            read.Close();
            return "No records exist";
        }

        public bool insertEntry(String userID, String name, String lastName, String email, String semiSecret1)
        {
            try
            {
                string query = String.Format("INSERT INTO users (`userID`, `name`, `lastname`, `email`, `semisecret1`) VALUES ('{0}', NOW(), 0, '{1}', 0);", userID, name, lastName, email, semiSecret1);
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

        public bool removeEntry(String email, String userID) //remove entry (complete)
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
