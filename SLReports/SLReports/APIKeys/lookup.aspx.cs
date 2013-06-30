using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.APIKeys
{
    public partial class lookup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblError.Text = "";
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            String keyText = txtKey.Text;

            litKeyInfo.Text = "";

            if (keyText.Length == 40)
            {
                lblError.Text = "";

                APIKey searchResults = null;

                String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    searchResults = APIKey.loadThisAPIKey(connection, keyText);
                }

                if (searchResults == null)
                {
                    lblError.Text = "Key not found";
                }
                else
                {
                    litKeyInfo.Text = "<br/><b>Key:</b> " + searchResults.key + "<br><b>Issued for</b> " + searchResults.username + "<br/><b>Description:</b>" + searchResults.description + "<br/><b>Date Issued:</b> " + searchResults.issueDate.ToShortDateString() + "<br/><b>Date Expires: </b>" + searchResults.expires.ToShortDateString();
                }

            }
            else
            {
                lblError.Text = "Invalid key";
            }

        }
    }
}