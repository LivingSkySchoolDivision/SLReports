using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class Nav : System.Web.UI.Page
    {

        public static List<NavMenuItem> getMainMenu()
        {
            List<NavMenuItem> returnMe = new List<NavMenuItem>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {
                returnMe = NavMenuItem.loadMenuItems(dbConnection);
            }
            returnMe.Sort();

            return returnMe;
        }

        private void Redirect(string url)
        {
            //Response.Write("Redirect to: " + url);
            Response.Redirect(url);
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            string serverURLPath = LSKYCommon.getServerURLPath(Request);

            if (Request.Form.AllKeys.Contains("selectedMenuItem"))
            {
                int selectedID = 0;

                if (int.TryParse(Request.Form["selectedMenuItem"], out selectedID))
                {
                    if (selectedID != 0)
                    {
                        List<NavMenuItem> NavMenu = getMainMenu();
                        foreach (NavMenuItem mi in NavMenu)
                        {
                            if (mi.id == selectedID)
                            {
                                //Response.Redirect(serverURLPath + mi.url);
                                Redirect(LSKYCommon.translateLocalURL(mi.url, Request));
                            }
                        }
                    }
                }                
            }

            Redirect(LSKYCommon.translateLocalURL("/", Request));
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}