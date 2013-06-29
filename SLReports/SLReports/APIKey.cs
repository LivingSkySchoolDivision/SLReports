using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SLReports
{
    public class APIKey
    {
        public string key { get; set; }
        public string username { get; set; }
        public DateTime issueDate { get; set; }
        public DateTime expires { get; set; }
        public bool internalOnly { get; set; }
        public string description { get; set; }

        public APIKey(string key, string username, string description, DateTime issuedate, DateTime expires, bool internalonly)
        {
            this.key = key;
            this.username = username;
            this.issueDate = issuedate;
            this.expires = expires;
            this.internalOnly = internalonly;
            this.description = description;
        }
        public static List<APIKey> loadAllAPIKeys(SqlConnection connection)
        {
            List<APIKey> returnMe = new List<APIKey>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM api_keys;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                                        returnMe.Add(new APIKey(
                            dataReader["api_key"].ToString().Trim(),
                            dataReader["username"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            DateTime.Parse(dataReader["date_issued"].ToString().Trim()),
                            DateTime.Parse(dataReader["date_expires"].ToString().Trim()),
                            bool.Parse(dataReader["is_internal_only"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
        public static List<APIKey> loadAPIKeysForUser(SqlConnection connection, string username)
        {
            List<APIKey> returnMe = new List<APIKey>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM api_keys WHERE username=@USERNAME;";
            sqlCommand.Parameters.AddWithValue("@USERNAME", username);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new APIKey(
                            dataReader["api_key"].ToString().Trim(),
                            dataReader["username"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            DateTime.Parse(dataReader["date_issued"].ToString().Trim()),
                            DateTime.Parse(dataReader["date_expires"].ToString().Trim()),
                            bool.Parse(dataReader["is_internal_only"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
        public static APIKey loadThisAPIKey(SqlConnection connection, string key)
        {
            APIKey returnMe = null;

            /* Make sure that the API key is valid - if not, do not attempt an SQL query with it */

            /* API keys are 40 characters */
            if (key.Length != 40)
            {
                return null;
            }

            /* Check for non alphanumeric characters */
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (!r.IsMatch(key))
            {
                return null;
            }

            /* TODO: figure out other ways to validate it... */

            /* Attempt to load the API key from the database */
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM api_keys WHERE api_key=@APIKEY;";
            sqlCommand.Parameters.AddWithValue("@APIKEY", key);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new APIKey(
                            dataReader["api_key"].ToString().Trim(),
                            dataReader["username"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            DateTime.Parse(dataReader["date_issued"].ToString().Trim()),
                            DateTime.Parse(dataReader["date_expires"].ToString().Trim()),
                            bool.Parse(dataReader["is_internal_only"].ToString().Trim())
                            );

                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
        public static bool deleteAPIKey(SqlConnection connection, string key)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "DELETE FROM api_keys WHERE api_key=@APIKEY;";
                sqlCommand.Parameters.AddWithValue("@APIKEY", key);
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Connection.Close();
                return true;
            }
            catch
            {
                return false;
            }        
        }

        public static string generateNewKeyString(string noise)
        {
            string salt = "Living Sky School Division: Salt Department";
            Random rand = new Random(DateTime.Now.Millisecond);
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.Year + "-" + DateTime.Now.Month + "." + DateTime.Now.Day + noise + salt + DateTime.Now + DateTime.Now.Millisecond));
            return BitConverter.ToString(hash).Replace("-","").Trim().ToLower();
        }

        public static void createAPIKey(SqlConnection connection, string username, bool is_internal_only, string description, string noise) 
        {
            /* Set the key to expire after a year - TODO: Revisit API key expiration dates*/
            DateTime expires = DateTime.Now.AddYears(1);

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "INSERT INTO api_keys(api_key,username,date_issued, date_expires, is_internal_only, description) VALUES(@APIKEY, @USERNAME, @ISSUEDATE, @EXPIRYDATE, @INTERNALONLY, @DESCRIPTION);";
                sqlCommand.Parameters.AddWithValue("@APIKEY", generateNewKeyString(description + DateTime.Now + username + noise));
                sqlCommand.Parameters.AddWithValue("@USERNAME", username);
                sqlCommand.Parameters.AddWithValue("@ISSUEDATE", DateTime.Today);
                sqlCommand.Parameters.AddWithValue("@EXPIRYDATE", expires);
                sqlCommand.Parameters.AddWithValue("@INTERNALONLY", is_internal_only);
                sqlCommand.Parameters.AddWithValue("@DESCRIPTION", description);
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Connection.Close();

            }
        }

    }
}