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
        private string loginURL = LSKYCommon.translateLocalURL("/Login/index.aspx");
        private List<NavMenuItem> MainMenu = null;
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
                if (!Request.ServerVariables["SCRIPT_NAME"].ToLower().Equals(loginURL.ToLower()))
                {                    
                    redirectToLogin();
                }
            }     
       
            /* Check to see if the the page is restricted to admins only */
            if (MainMenu == null)
            {
                MainMenu = Nav.getMainMenu();
            }
            List<string> restrictedPages = new List<string>();

            foreach (NavMenuItem item in MainMenu)
            {
                if (item.admin_only)
                {
                    restrictedPages.Add(item.url);
                }
            }
            
            bool grantAccess = true;
            foreach (string restrictedURL in restrictedPages) 
            {
                if (Request.RawUrl.ToLower().Contains(restrictedURL.ToLower()))
                {
                    if (!loggedInUser.is_admin)
                    {
                        grantAccess = false;
                    }                    
                }
            }

            if (!grantAccess)
            {
                Response.Write("Your user account does not have access to this page.");
                Response.End();
            }
        }

        public void displayNavDropdown()
        {
            /* Load the menu items */
            if (MainMenu == null)
            {
                MainMenu = Nav.getMainMenu();
            }

            // Figure out which menu items to display
            List<NavMenuItem> displayedMenuItems = new List<NavMenuItem>();            
            foreach (NavMenuItem item in MainMenu)
            {
                if (!item.hidden)
                {

                    if (loggedInUser.is_admin)
                    {
                        displayedMenuItems.Add(item);
                    }
                    else
                    {
                        if ((!item.admin_only) && (!item.hidden))
                        {
                            displayedMenuItems.Add(item);
                        }
                    }
                }
            }            
            displayedMenuItems.Sort();

            // Figure out which categories to display
            List<string> MenuCategories = new List<string>();
            foreach (NavMenuItem item in displayedMenuItems)
            {
                if (!MenuCategories.Contains(item.category))
                {
                    MenuCategories.Add(item.category);
                }
            }


            // Display the list in a dropdown box
            Response.Write("<form method=\"post\" action=\"" + LSKYCommon.translateLocalURL("Nav.aspx") + "\" style=\"margin: 0; padding: 0;\">");
            Response.Write("<span class=\"nav_link\">Navigation:</span> ");
            Response.Write("<select name=\"selectedMenuItem\">");
            Response.Write("<option value=\"0\"> -- Front Page --</option>");
            foreach (string MenuCat in MenuCategories)
            {
                Response.Write("<option value=\"0\" style=\"font-weight: bold;\">" + MenuCat.ToUpper() + "</option>");
                foreach (NavMenuItem mi in displayedMenuItems)
                {
                    if (mi.category == MenuCat)
                    {
                        Response.Write("<option value=\"" + mi.id + "\">&nbsp;&nbsp;&nbsp;" + mi.name + "</option>");
                    }
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