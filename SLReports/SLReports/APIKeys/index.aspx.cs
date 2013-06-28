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
    public partial class index : System.Web.UI.Page
    {
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

        private TableRow keyTableHeaders()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell cell_username = new TableCell();
            cell_username.Text = "Username";
            cell_username.Width = 150;
            newRow.Cells.Add(cell_username);

            TableCell cell_key = new TableCell();
            cell_key.Text = "Key";
            cell_key.Width = 300;
            newRow.Cells.Add(cell_key);

            TableCell cell_issuedate = new TableCell();
            cell_issuedate.Text = "Issued";
            cell_issuedate.Width = 150;
            newRow.Cells.Add(cell_issuedate);

            TableCell cell_expirydate = new TableCell();
            cell_expirydate.Text = "Expires";
            cell_expirydate.Width = 150;
            newRow.Cells.Add(cell_expirydate);

            TableCell cell_description = new TableCell();
            cell_description.Text = "Description";
            cell_description.Width = 250;
            newRow.Cells.Add(cell_description);
            return newRow;
        }

        private TableRow keyTableRow(APIKey key)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "Row";

            TableCell cell_username = new TableCell();
            cell_username.Text = key.username;
            newRow.Cells.Add(cell_username);
            
            TableCell cell_key = new TableCell();
            cell_key.Text = key.key;
            newRow.Cells.Add(cell_key);

            TableCell cell_issuedate = new TableCell();
            cell_issuedate.Text = key.issueDate.ToShortDateString();
            newRow.Cells.Add(cell_issuedate);

            TableCell cell_expirydate = new TableCell();
            cell_expirydate.Text = key.expires.ToShortDateString();
            newRow.Cells.Add(cell_expirydate);

            TableCell cell_description = new TableCell();
            cell_description.Text = key.description;
            newRow.Cells.Add(cell_description);
            return newRow;
        }

        private APIKey anonymizeKey(APIKey thisKey)
        {
            return new APIKey("- NOT AVAILABLE-", thisKey.username, thisKey.description, thisKey.issueDate, thisKey.expires, thisKey.internalOnly);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            session loggedInUser = null;
            List<APIKey> userKeys = new List<APIKey>();
            List<APIKey> allKeys = new List<APIKey>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                loggedInUser = session.loadThisSession(connection, getSessionIDFromCookies(), Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"]);
                userKeys = APIKey.loadAPIKeysForUser(connection, loggedInUser.getUsername());
                allKeys = APIKey.loadAllAPIKeys(connection);
            }

            tblKeys.Rows.Add(keyTableHeaders());
            foreach (APIKey key in userKeys)
            {
                tblKeys.Rows.Add(keyTableRow(key));
            }

            tblAllKeys.Rows.Add(keyTableHeaders());
            foreach (APIKey key in allKeys)
            {
                tblAllKeys.Rows.Add(keyTableRow(anonymizeKey(key)));
            }



            
        }
    }
}