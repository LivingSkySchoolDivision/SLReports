using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Logs
{
    public partial class index : System.Web.UI.Page
    {
        List<session> AllSessions = null;
        List<LoginAttempt> AllLoginAttempts = null;

        private List<LoginAttempt> getLoginAttempts(DateTime from, DateTime to)
        {
            List<LoginAttempt> returnMe = new List<LoginAttempt>();

            try
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString; 
                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = dbConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "SELECT * FROM audit_loginAttempts WHERE eventTime < @EventTo AND eventTime > @EventFrom ORDER BY eventTime ASC;";
                        sqlCommand.Parameters.AddWithValue("@EventTo", to);
                        sqlCommand.Parameters.AddWithValue("@EventFrom", from);

                        sqlCommand.Connection.Open();
                        SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                        if (dbDataReader.HasRows)
                        {
                            while (dbDataReader.Read())
                            {
                                returnMe.Add(new LoginAttempt(
                                    DateTime.Parse(dbDataReader["eventTime"].ToString()),
                                    dbDataReader["enteredUsername"].ToString(),
                                    dbDataReader["ipaddress"].ToString(),
                                    dbDataReader["useragent"].ToString(),
                                    dbDataReader["status"].ToString(),
                                    dbDataReader["info"].ToString()
                                    ));
                            }
                        }
                        sqlCommand.Connection.Close();
                    }
                }
            }
            catch (Exception e) { Response.Write(e.Message); }
            return returnMe;

        }

        private List<session> getActiveSessions()
        {
            List<session> returnMe = new List<session>();

            try
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString; 
                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = dbConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "SELECT * FROM sessions WHERE sessionstarts < {fn NOW()} AND sessionends > {fn NOW()};";

                        sqlCommand.Connection.Open();
                        SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                        if (dbDataReader.HasRows)
                        {
                            while (dbDataReader.Read())
                            {
                                returnMe.Add(new session(
                                    dbDataReader["username"].ToString(),
                                    dbDataReader["ip"].ToString(),
                                    dbDataReader["id_hash"].ToString(),
                                    dbDataReader["useragent"].ToString(),
                                    DateTime.Parse(dbDataReader["sessionstarts"].ToString()),
                                    DateTime.Parse(dbDataReader["sessionends"].ToString())
                                    ));
                            }
                        }
                        sqlCommand.Connection.Close();
                    }
                }
            }
            catch (Exception e) { Response.Write(e.Message); }
            return returnMe;
        }

        private session getSession(string hash, string ip, string useragent)
        {
            session returnme = null;

            /* Search for the session hash in the database */
            try
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString; 
                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = dbConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "SELECT * FROM sessions WHERE id_hash=@Hash AND ip=@IP AND useragent=@UA;";
                        sqlCommand.Parameters.AddWithValue("@Hash", hash);
                        sqlCommand.Parameters.AddWithValue("@IP", ip);
                        sqlCommand.Parameters.AddWithValue("@UA", useragent);

                        sqlCommand.Connection.Open();
                        SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                        if (dbDataReader.HasRows)
                        {
                            while (dbDataReader.Read())
                            {
                                returnme = new session(
                                    dbDataReader["username"].ToString(),
                                    dbDataReader["ip"].ToString(),
                                    dbDataReader["id_hash"].ToString(),
                                    dbDataReader["useragent"].ToString(),
                                    DateTime.Parse(dbDataReader["sessionstarts"].ToString()),
                                    DateTime.Parse(dbDataReader["sessionends"].ToString())
                                    );
                            }
                        }
                        sqlCommand.Connection.Close();
                    }
                }
            }
            catch { }

            if (returnme != null)
            {
                if (returnme.getIP().Equals(ip))
                {
                    if (returnme.getUserAgent().Equals(useragent))
                    {
                        if (returnme.getStart() < DateTime.Now)
                        {
                            if (returnme.getEnd() > DateTime.Now)
                            {
                                return returnme;
                            }
                        }
                    }
                }
            }

            return null;
        }
        private string getSessionIDFromCookies()
        {
            HttpCookie sessionCookie = Request.Cookies["lskyDataExplorer"];
            if (sessionCookie != null)
            {
                return sessionCookie.Value;
            }
            else
            {
                return null;
            }
        }

        private TableRow addLoginAttemptRow(LoginAttempt thisLoginAttempt)
        {
            TableRow returnMe = new TableRow();
            TableCell cell_time = new TableCell();
            TableCell cell_username = new TableCell();
            TableCell cell_ip = new TableCell();
            TableCell cell_info = new TableCell();

            cell_time.Text = thisLoginAttempt.eventTime.ToShortDateString() + " " + thisLoginAttempt.eventTime.ToLongTimeString();
            cell_username.Text = thisLoginAttempt.enteredUserName;
            cell_ip.Text = thisLoginAttempt.ipAddress;
            cell_info.Text = thisLoginAttempt.info;

            returnMe.Cells.Add(cell_time);
            returnMe.Cells.Add(cell_username);
            returnMe.Cells.Add(cell_ip);
            returnMe.Cells.Add(cell_info);

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load sessions */
            AllSessions = getActiveSessions();
            AllLoginAttempts = getLoginAttempts(DateTime.Now.AddMonths(-1), DateTime.Now);

            #region load sessions into table
            foreach (session ses in AllSessions)
            {
                tblSessions.CellPadding = 3;
                TableRow tblRow = new TableRow();
                tblRow.CssClass = "datatable";
                TableCell cell_username = new TableCell();
                TableCell cell_starttime = new TableCell();
                TableCell cell_endtime = new TableCell();
                TableCell cell_IP = new TableCell();
                //TableCell cell_Hash = new TableCell();
                //TableCell cell_Agent = new TableCell();

                cell_username.Text = ses.getUsername();
                cell_starttime.Text = ses.getStart().ToLongDateString() + " " + ses.getStart().ToLongTimeString();
                cell_endtime.Text = ses.getEnd().ToLongDateString() + " " + ses.getEnd().ToLongTimeString();
                cell_IP.Text = ses.getIP();
                //cell_Hash.Text = ses.getHash();
                //cell_Agent.Text = ses.getUserAgent();

                if (ses.getHash().Equals(getSessionIDFromCookies()))
                {
                    cell_username.Text += " (You)";
                    cell_username.Font.Bold = true;
                    cell_starttime.Font.Bold = true;
                    cell_endtime.Font.Bold = true;
                    cell_IP.Font.Bold = true;
                }


                tblRow.Cells.Add(cell_username);
                tblRow.Cells.Add(cell_starttime);
                tblRow.Cells.Add(cell_endtime);
                tblRow.Cells.Add(cell_IP);
                //tblRow.Cells.Add(cell_Hash);
                //tblRow.Cells.Add(cell_Agent);

                tblSessions.Rows.Add(tblRow);
            }
            #endregion

            #region load log into table



            /* Successful login attempts */
            foreach (LoginAttempt la in AllLoginAttempts)
            {
                if (la.status.ToLower().Equals("success"))
                {
                    tblLogins_Success.Rows.Add(addLoginAttemptRow(la));
                }
            }

            /* Unsuccessful login attempts */
            foreach (LoginAttempt la in AllLoginAttempts)
            {
                if (la.status.ToLower().Equals("denied"))
                {
                    tblLogins_Failure.Rows.Add(addLoginAttemptRow(la));
                }
            }
            #endregion

        }
    }
}