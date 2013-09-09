using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Logs
{
    public partial class WhoHasAccess : System.Web.UI.Page
    {     
        TableRow addUserRow(string username)
        {
            TableRow newRow = new TableRow();

            TableCell usernameCell = new TableCell();
            usernameCell.Text = username;
            newRow.Cells.Add(usernameCell);


            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //lblAdminGroup.Text = LSKYCommon.adminGroupName;
            //lblUserGroup.Text = LSKYCommon.userGroupName;

            // Get a list of users from security groups - recursively
            List<string> groupMembers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.userGroupName);
            List<string> adminGroupMembers = LSKYCommon.getGroupMembers("lskysd", LSKYCommon.adminGroupName);

            groupMembers.Sort();
            adminGroupMembers.Sort();

            foreach (string user in adminGroupMembers)
            {
                tblAdministrators.Rows.Add(addUserRow(user));
            }
            
            foreach (string user in groupMembers)
            {
                tblUsers.Rows.Add(addUserRow(user));
            }
            

        }
    }
}