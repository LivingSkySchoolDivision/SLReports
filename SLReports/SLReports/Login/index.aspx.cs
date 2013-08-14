using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Login
{
    public partial class index : System.Web.UI.Page
    {
        

        public bool validate(string domain, string username, string pwd)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                return pc.ValidateCredentials(username, pwd);
            }

        }

        public string getMD5(string input)
        {
            string returnMe = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }


        }

        private string getNewSessionID(string salt)
        {
            return getMD5(DateTime.Now.ToString("ffffff") + salt);
        }

        private void logLoginAttempt(string username, string remoteIP, string useragent, string status, string info)
        {
            try
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString; 
                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = dbConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "INSERT INTO audit_loginAttempts(eventTime,enteredUsername,ipaddress,useragent,status,info) VALUES(@CurrentTime, @Username, @IP, @UserAgent, @Status, @Info);";
                        sqlCommand.Parameters.AddWithValue("@CurrentTime", DateTime.Now.ToString());
                        sqlCommand.Parameters.AddWithValue("@Username", username);
                        sqlCommand.Parameters.AddWithValue("@IP", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@UserAgent", useragent);
                        sqlCommand.Parameters.AddWithValue("@Status", status);
                        sqlCommand.Parameters.AddWithValue("@Info", info);
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Connection.Close();
                    }
                }
            }
            catch (Exception ex) 
            {
                Response.Write("Exception: " + ex.Message);
            }
        }

        private void createSession(string username, string remoteIP, string useragent, bool is_admin)
        {
            string newSessionID = getNewSessionID(Request.ServerVariables["ALL_RAW"]);

            /* Create a session in the database */

            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;

            /* Set a limit on how long this login session will last, based on time of day */
            /*  If logging in during the work day, make a session last 7 hours */
            /*  If logging in after hours, make the session only last 2 hours */
            TimeSpan workDayStart = new TimeSpan(7, 0, 0);
            TimeSpan workDayEnd = new TimeSpan(16, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan sessionDuration;

            if ((now >= workDayStart) && (now <= workDayEnd))
            {
                sessionDuration = new TimeSpan(8, 0, 0);
            }
            else
            {
                sessionDuration = new TimeSpan(2, 0, 0);
            }

            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    //sqlCommand.CommandText = "INSERT INTO sessions(id_hash,username,ip,useragent,sessionstarts,sessionends,is_admin) VALUES('" + newSessionID + "','" + username + "','" + remoteIP + "','" + useragent + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.Add(sessionDuration).ToString() + "');";
                    
                    sqlCommand.CommandText = "INSERT INTO sessions(id_hash,username,ip,useragent,sessionstarts,sessionends) VALUES(@ID, @USERNAME, @IP, @USERAGENT, @SESSIONSTART, @SESSIONEND);";
                    sqlCommand.Parameters.AddWithValue("@ID", newSessionID);
                    sqlCommand.Parameters.AddWithValue("@USERNAME", username);
                    sqlCommand.Parameters.AddWithValue("@IP", remoteIP);
                    sqlCommand.Parameters.AddWithValue("@USERAGENT", useragent);
                    sqlCommand.Parameters.AddWithValue("@SESSIONSTART", DateTime.Now);
                    sqlCommand.Parameters.AddWithValue("@SESSIONEND", DateTime.Now.Add(sessionDuration));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }

            /* Create a cookie with the hashed ID */
            HttpCookie newCookie = new HttpCookie("lskyDataExplorer");
            newCookie.Value = newSessionID;
            newCookie.Expires = DateTime.Now.Add(sessionDuration);
            newCookie.Domain = "sldata.lskysd.ca";
            newCookie.Secure = true;
            Response.Cookies.Add(newCookie);

        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        /* This doesn't do anything yet - It was a hair-brained scheme that may or may not make it into the final version */
        private bool isPasswordStrongEnough(string thisPassword)
        {
            bool returnme = false;

            char[] upperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] lowerCase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] specialchar = { ' ', '.', ',', '!', '@', '#', '$', '%', '^', '&', '\\', '/', '*', '(', ')', '-', '+', '|', '?', '`', '~' };
            
            return returnme;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string givenUsername = txtUsername.Text;
            string givenPassword = txtPassword.Text;


            List<string> groupMembers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.userGroupName);
            List<string> adminGroupMembers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.adminGroupName);

            if (!((string.IsNullOrEmpty(givenUsername)) || (string.IsNullOrEmpty(givenPassword))))
            {
                if (validate("lskysd", givenUsername, givenPassword))
                {
                    if (adminGroupMembers.Contains(givenUsername))
                    {
                        logLoginAttempt(txtUsername.Text, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "SUCCESS", "User is administrator");
                        createSession(givenUsername, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], true);
                                                
                        Response.Redirect("/SLReports/");
                    } 
                    else if (groupMembers.Contains(givenUsername))
                    {

                        logLoginAttempt(txtUsername.Text, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "SUCCESS", "");
                        createSession(givenUsername, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], false);

                        Response.Redirect("/SLReports/");
                    }
                    else
                    {
                        lblStatus.ForeColor = Color.Red;
                        lblStatus.Text = "Access Denied";
                        logLoginAttempt(txtUsername.Text, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "DENIED", "User is not in security group");
                    }
                }
                else
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Access Denied";
                    logLoginAttempt(txtUsername.Text, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "DENIED", "Incorrect username or password");
                }
            }                
        }
    }
}