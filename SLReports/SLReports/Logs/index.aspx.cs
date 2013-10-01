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
        private const int recordsToDisplay = 50;

        private List<LoginAttempt> getLoginAttempts(SqlConnection connection, DateTime from, DateTime to)
        {
            List<LoginAttempt> returnMe = new List<LoginAttempt>();

            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "SELECT TOP 100 * FROM audit_loginAttempts WHERE eventTime < @EventTo AND eventTime > @EventFrom ORDER BY eventTime DESC;";
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
            catch (Exception e) { Response.Write(e.Message); }
            return returnMe;
        }
        
        private session getSession(string hash, string ip, string useragent)
        {
            session returnme = null;

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString; 
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                returnme = session.loadThisSession(connection, hash, ip, useragent);
            }

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
            returnMe.CssClass = "datatable_row";

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

        private TableRow addLoginAttemptRowWithType(LoginAttempt thisLoginAttempt)
        {
            System.Drawing.Color bgColor = System.Drawing.Color.LightGray;
            if (thisLoginAttempt.status.ToLower().Equals("success"))
            {
                bgColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                bgColor = System.Drawing.Color.LightSalmon;
            }


            TableRow returnMe = new TableRow();
            returnMe.CssClass = "datatable_row";

            TableCell cell_type = new TableCell();
            cell_type.BackColor = bgColor;
            TableCell cell_time = new TableCell();
            cell_time.BackColor = bgColor;
            TableCell cell_username = new TableCell();
            cell_username.BackColor = bgColor;
            TableCell cell_ip = new TableCell();
            cell_ip.BackColor = bgColor;
            TableCell cell_info = new TableCell();
            cell_info.BackColor = bgColor;

            TableCell cell_UserAgent = new TableCell();
            cell_UserAgent.BackColor = bgColor;

            
            
            cell_type.Text = thisLoginAttempt.status;
            cell_time.Text = thisLoginAttempt.eventTime.ToShortDateString() + " " + thisLoginAttempt.eventTime.ToLongTimeString();
            cell_username.Text = thisLoginAttempt.enteredUserName;
            cell_ip.Text = thisLoginAttempt.ipAddress;
            cell_info.Text = thisLoginAttempt.info;
            cell_UserAgent.Text = thisLoginAttempt.userAgent;

            returnMe.Cells.Add(cell_type);
            returnMe.Cells.Add(cell_time);
            returnMe.Cells.Add(cell_username);
            returnMe.Cells.Add(cell_ip);
            returnMe.Cells.Add(cell_info);
            returnMe.Cells.Add(cell_UserAgent);

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load sessions */
            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                AllSessions = session.loadActiveSessions(connection);
                AllLoginAttempts = getLoginAttempts(connection, DateTime.Now.AddMonths(-1), DateTime.Now);
            }         

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

                if (ses.is_admin)
                {
                    cell_username.Text += " <b style=\"color: red;\">(ADMIN)</b>";
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

            int counter = 0;


            /* All login attempts */
            /*
            int numToDisplay = recordsToDisplay;
            if (AllLoginAttempts.Count < recordsToDisplay)
            {
                numToDisplay = AllLoginAttempts.Count;
            }

            Response.Write("<BR>Displaying: " + numToDisplay);

            for (int x = AllLoginAttempts.Count; x < (AllLoginAttempts.Count - numToDisplay); x--) 
            {
                Response.Write(x + ", ");
                tblLogins_All.Rows.Add(addLoginAttemptRowWithType(AllLoginAttempts[x]));
            }
            */

            foreach (LoginAttempt la in AllLoginAttempts)
            {
                tblLogins_All.Rows.Add(addLoginAttemptRowWithType(la));
            }
                

            /* Successful login attempts */
            counter = 0;
            foreach (LoginAttempt la in AllLoginAttempts)
            {
                counter++;
                if (counter <= recordsToDisplay)
                {
                    if (la.status.ToLower().Equals("success"))
                    {
                        tblLogins_Success.Rows.Add(addLoginAttemptRow(la));
                    }
                }
            }

            /* Unsuccessful login attempts */
            counter = 0;
            foreach (LoginAttempt la in AllLoginAttempts)
            {
                counter++;
                if (counter <= recordsToDisplay)
                {
                    if (la.status.ToLower().Equals("denied"))
                    {
                        tblLogins_Failure.Rows.Add(addLoginAttemptRow(la));
                    }
                }
            }
            #endregion

        }
    }
}