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

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "SELECT * FROM menu_items;";
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            returnMe.Add(new NavMenuItem(
                                int.Parse(dbDataReader["id"].ToString()),
                                int.Parse(dbDataReader["parent"].ToString()),
                                dbDataReader["url"].ToString(),
                                dbDataReader["name"].ToString(),
                                dbDataReader["description"].ToString()
                                ));
                        }
                    }

                }
            }

            return returnMe;
        }
               


        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.Form.AllKeys.Contains("selectedMenuItem"))
            {
                int selectedID = int.Parse(Request.Form["selectedMenuItem"]);
                List<NavMenuItem> NavMenu = getMainMenu();
                foreach (NavMenuItem mi in NavMenu)
                {
                    if (mi.id == selectedID)
                    {
                        Response.Redirect("/SLReports" + mi.url);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}