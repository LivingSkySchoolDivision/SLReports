using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
            if (loggedInUser != null)
            {                
                String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    session.expireSession(dbConnection, loggedInUser.getHash());
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
            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
            APIKey apiKey = null;

            /* Check for an API key */
            if (!string.IsNullOrEmpty(Request.QueryString["apikey"]))
            {
                Response.Write("Derp");
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    apiKey = APIKey.loadThisAPIKey(connection,Request.QueryString["apikey"]);
                }

                if (apiKey != null)
                {
                    if (apiKey.internalOnly)
                    {                                                
                        if (
                            !(
                                (Request.ServerVariables["REMOTE_ADDR"].Contains("127.0.0.1")) ||
                                (Request.ServerVariables["REMOTE_ADDR"].Contains("::1"))
                             )
                           )
                        {
                            apiKey = null;
                        }
                    }
                }

                /* If they key is used, log it */
                if (apiKey != null)
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        APIKey.logAPIKeyUse(connection, apiKey, Request.ServerVariables["UNENCODED_URL"], Request.ServerVariables["HTTP_USER_AGENT"], Request.ServerVariables["REMOTE_ADDR"]);
                    }
                }
            }

            /* Check for a username */
            if (!string.IsNullOrEmpty(getSessionIDFromCookies()))
            {
                 using (SqlConnection connection = new SqlConnection(dbConnectionString))
                 {
                     loggedInUser = session.loadThisSession(connection, getSessionIDFromCookies(), Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"]);
                 }
            }

            if ((loggedInUser == null) && (apiKey == null))
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
                if (mi.admin_only)
                {
                    if (loggedInUser.is_admin)
                    {
                        Response.Write("<option value=\"" + mi.id + "\">" + mi.name + "</option>");
                    }
                }
                else if (mi.hidden)
                {
                    if (loggedInUser.is_admin)
                    {
                        Response.Write("<option value=\"" + mi.id + "\">" + mi.name + "</option>");
                    }
                }
                else
                {
                    Response.Write("<option value=\"" + mi.id + "\">" + mi.name + "</option>");
                }
            }
            Response.Write("</select>");
            Response.Write("&nbsp;<input type=\"submit\" value=\"Go\">");
            Response.Write("</form>");
        }

        public void displayUserBanner()
        {
            if (loggedInUser != null)
            {
                Response.Write("<div id=\"loggedInUserBanner\">Logged in as <b>" + loggedInUser.getUsername() + "</b>");
                if (loggedInUser.is_admin)
                {
                    Response.Write(" <b style=\"color: red;\">(ADMIN)</b>");
                }
                Response.Write("<br>Session expires <b>" + loggedInUser.getEnd().ToShortDateString() + " " + loggedInUser.getEnd().ToShortTimeString() + "</b><br><b><a href=\"?logout=true\">Click here to log out</a></b></div>");
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