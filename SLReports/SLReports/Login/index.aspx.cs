using System;
using System.Collections.Generic;
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
        /* TODO: Don't store this in the code - figure out how I should be storing this information */
        String dbUser = @"data_explorer";
        String dbPassword = @"YKy08UJBBbwOoktJ";
        String dbHost = "localhost";
        String dbDatabase = "DataExplorer";

        public List<String> getGroupMembers(string domain, string groupName)
        {
            List<string> returnMe = new List<string>();

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                using (GroupPrincipal grp = GroupPrincipal.FindByIdentity(pc,IdentityType.Name,groupName))
                {
                    if (grp != null)
                    {
                        foreach (Principal p in grp.GetMembers(true))
                        {
                            returnMe.Add(p.SamAccountName);
                        }
                    }
                }
            }
            return returnMe;    
        }

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
                String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
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

        private void createSession(string username, string remoteIP, string useragent)
        {
            string newSessionID = getNewSessionID(Request.ServerVariables["ALL_RAW"]);

            /* Create a session in the database */

            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";


            /* Sessions last this long                                 */
            /*                                      /- Hours            */
            /*                                      |  /- Minutes       */
            /*                                      |  |  /- Seconds    */
            TimeSpan sessionDuration = new TimeSpan(8, 0, 0);

            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "INSERT INTO sessions(id_hash,username,ip,useragent,sessionstarts,sessionends) VALUES('" + newSessionID + "','" + username + "','" + remoteIP + "','" + useragent + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.Add(sessionDuration).ToString() + "');";

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

            string groupName = "SchoolLogicDataExplorerUsers";

            List<string> groupMembers = getGroupMembers("lskysd", groupName);

            if (validate("lskysd", givenUsername, givenPassword))
            {
                if (groupMembers.Contains(givenUsername))
                {
                    logLoginAttempt(txtUsername.Text, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "SUCCESS", "");
                    createSession(givenUsername, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"]);

                    /* Redirect somewhere else maybe */
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