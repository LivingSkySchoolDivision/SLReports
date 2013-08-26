using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class ServerVariables : System.Web.UI.Page
    {

        private TableRow addRow(string key, string value)
        {
            TableRow newRow = new TableRow();

            TableCell nameCell = new TableCell();
            nameCell.BorderWidth = 1;
            nameCell.Text = key;

            TableCell valueCell = new TableCell();
            valueCell.BorderWidth = 1;
            valueCell.Text = value;

            newRow.Cells.Add(nameCell);
            newRow.Cells.Add(valueCell);
            return newRow;
        }

        

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (string var in Request.ServerVariables) 
            {
                tblVars.Rows.Add(addRow(var, Request.ServerVariables[var]));
            }


            

            lblTest.Text = LSKYCommon.getServerURLPath(Request);


            
        }
    }
}