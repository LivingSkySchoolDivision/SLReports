﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class BasicTemplate : System.Web.UI.MasterPage
    {

        private string loginURL = "/SLReports/Login/index.aspx";
        private List<NavMenuItem> MainMenu;

        public session loggedInUser = null;

        private session getSession(string hash, string ip, string useragent)
        {
            session returnme = null;

            /* Search for the session hash in the database */
            try
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
                
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
        public void expireSession()
        {
            /* Remove the session from the server */
            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
            
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "DELETE FROM sessions WHERE id_hash=@Hash;";                    
                    sqlCommand.Parameters.AddWithValue("@Hash", loggedInUser.getHash());
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }

            /* Expire the cookie on the client side */
            if (Request.Cookies.AllKeys.Contains("lskyDataExplorer"))
            {
                HttpCookie newCookie = new HttpCookie("lskyDataExplorer");
                newCookie.Value = "NOTHING TO SEE HERE";
                newCookie.Expires = DateTime.Now.AddDays(-1D);
                newCookie.Domain = "sldata.lskysd.ca";
                newCookie.Secure = true;
                //Response.SetCookie(Response.Cookies["lskyDataExplorer"]);
                Response.Cookies.Add(newCookie);
            }

            /* Get rid of the object in memory... just in case...*/
            loggedInUser = null;

            /* Redirect to the login page */
            redirectToLogin();           
        }

        public void redirectToLogin()
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Redirect(loginURL); 
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(getSessionIDFromCookies()))
            {
                loggedInUser = getSession(getSessionIDFromCookies(), Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"]);
            }

            if (loggedInUser == null)
            {
                if (!Request.ServerVariables["SCRIPT_NAME"].Equals(loginURL))
                {                    
                    redirectToLogin();
                }
            }
        }

        public void displayNavDropdown()
        {
            /* Load the menu items */
            MainMenu = Nav.getMainMenu();
            Response.Write("<form method=\"post\" action=\"/SLReports/Nav.aspx\" style=\"margin: 0; padding: 0;\">");
            Response.Write("<span class=\"nav_link\">Navigation:</span> ");
            Response.Write("<select name=\"selectedMenuItem\">");
            MainMenu.Sort();
            foreach (NavMenuItem mi in MainMenu)
            {
                Response.Write("<option value=\"" + mi.id + "\">" + mi.name + "</option>");
            }
            Response.Write("</select>");
            Response.Write("&nbsp;<input type=\"submit\" value=\"Go\">");
            Response.Write("</form>");
        }

        public void displayUserBanner()
        {
            if (loggedInUser != null)
            {
                Response.Write("<div id=\"loggedInUserBanner\">Logged in as <b>" + loggedInUser.getUsername() + "</b><br>Session expires <b>" + loggedInUser.getEnd().ToShortDateString() + " " + loggedInUser.getEnd().ToShortTimeString() + "</b><br><b><a href=\"?logout=true\">Click here to log out</a></b></div>");
            }
            else
            {
                if (!Request.ServerVariables["SCRIPT_NAME"].Equals(loginURL))
                {
                    redirectToLogin();
                }
            }
        }       


        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Request.QueryString.AllKeys.Contains("logoff")) || (Request.QueryString.AllKeys.Contains("logout")))
            {                
                expireSession();                
            }
        }        
    }
}