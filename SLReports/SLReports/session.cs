using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class session
    {
        private string username;
        private string ip;
        private string hash;
        private string useragent;
        private DateTime starts;
        private DateTime ends;

        public bool is_admin { get; set; }        

        public session(string username, string ip, string hash, string useragent, DateTime starts, DateTime ends, bool is_admin)
        {
            this.username = username;
            this.ip = ip;
            this.hash = hash;
            this.useragent = useragent;
            this.starts = starts;
            this.ends = ends;
            this.is_admin = is_admin;
        }

        public string getUsername() 
        {
            return username;
        }

        public string getIP()
        {
            return ip;
        }

        public string getHash()
        {
            return hash;
        }

        public string getUserAgent()
        {
            return useragent;
        }

        public DateTime getStart()
        {
            return starts;
        }

        public DateTime getEnd()
        {
            return ends;
        }

        public static List<session> loadAllSessions(SqlConnection connection)
        {
            List<session> returnMe = new List<session>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM sessions;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                List<string> adminUsers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.adminGroupName);
                
                while (dataReader.Read())
                {
                    bool is_admin = false;
                    if (adminUsers.Contains(dataReader["username"].ToString()))
                    {
                        is_admin = true;
                    }

                    returnMe.Add(new session(
                                dataReader["username"].ToString(),
                                dataReader["ip"].ToString(),
                                dataReader["id_hash"].ToString(),
                                dataReader["useragent"].ToString(),
                                DateTime.Parse(dataReader["sessionstarts"].ToString()),
                                DateTime.Parse(dataReader["sessionends"].ToString()),
                                is_admin
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static session loadThisSession(SqlConnection connection, string hash, string ip, string useragent)
        {
            session returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM sessions WHERE id_hash=@Hash AND ip=@IP AND useragent=@UA;";
            sqlCommand.Parameters.AddWithValue("@Hash", hash);
            sqlCommand.Parameters.AddWithValue("@IP", ip);
            sqlCommand.Parameters.AddWithValue("@UA", useragent);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                List<string> adminUsers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.adminGroupName);

                while (dataReader.Read())
                {
                    bool is_admin = false;
                    if (adminUsers.Contains(dataReader["username"].ToString()))
                    {
                        is_admin = true;
                    }

                    returnMe = new session(
                                dataReader["username"].ToString(),
                                dataReader["ip"].ToString(),
                                dataReader["id_hash"].ToString(),
                                dataReader["useragent"].ToString(),
                                DateTime.Parse(dataReader["sessionstarts"].ToString()),
                                DateTime.Parse(dataReader["sessionends"].ToString()),
                                is_admin
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static bool expireSession(SqlConnection connection, string hash)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "DELETE FROM sessions WHERE id_hash=@Hash;";
                sqlCommand.Parameters.AddWithValue("@Hash", hash);
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

        public static List<session> loadActiveSessions(SqlConnection connection)
        {
            List<session> returnMe = new List<session>();

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM sessions WHERE sessionstarts < {fn NOW()} AND sessionends > {fn NOW()};";

                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    List<string> adminUsers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.adminGroupName);

                    while (dbDataReader.Read())
                    {
                        bool is_admin = false;
                        if (adminUsers.Contains(dbDataReader["username"].ToString()))
                        {
                            is_admin = true;
                        }

                        returnMe.Add(new session(
                            dbDataReader["username"].ToString(),
                            dbDataReader["ip"].ToString(),
                            dbDataReader["id_hash"].ToString(),
                            dbDataReader["useragent"].ToString(),
                            DateTime.Parse(dbDataReader["sessionstarts"].ToString()),
                            DateTime.Parse(dbDataReader["sessionends"].ToString()),
                            is_admin
                            ));
                    }
                }
                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

    }
}