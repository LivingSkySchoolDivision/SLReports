using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Staff
{
    public partial class index : System.Web.UI.Page
    {

        protected TableRow addTitleTableRow(School school)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell schoolNameCell = new TableCell();
            schoolNameCell.Text = school.getName();
            schoolNameCell.ColumnSpan = 4;
            newRow.Cells.Add(schoolNameCell);

            return newRow;

        }

        protected TableRow addHeaderTableRow()
        {

            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell firstNameCell = new TableCell();
            firstNameCell.Text = "First Name";
            firstNameCell.Width = 150;
            newRow.Cells.Add(firstNameCell);

            TableCell lastNameCell = new TableCell();
            lastNameCell.Text = "Last Name";
            lastNameCell.Width = 150;
            newRow.Cells.Add(lastNameCell);

            TableCell ldapNameCell = new TableCell();
            ldapNameCell.Text = "LDAP Name";
            ldapNameCell.Width = 150;
            newRow.Cells.Add(ldapNameCell);

            return newRow;
        }

        protected TableRow addStaffTableRow(StaffMember staff)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell firstNameCell = new TableCell();
            firstNameCell.Text = staff.givenName;
            newRow.Cells.Add(firstNameCell);

            TableCell lastNameCell = new TableCell();
            lastNameCell.Text = staff.sn;
            newRow.Cells.Add(lastNameCell);

            TableCell ldapNameCell = new TableCell();
            ldapNameCell.Text = staff.LDAPName;
            newRow.Cells.Add(ldapNameCell);

            return newRow;

        }

        protected Table buildStaffTable(School school, List<StaffMember> allStaff)
        {
            Table newTable = new Table();
            newTable.CssClass = "datatable";
            
            newTable.Rows.Add(addTitleTableRow(school));
            newTable.Rows.Add(addHeaderTableRow());

            foreach (StaffMember staff in allStaff)
            {
                if (staff.schoolID == int.Parse(school.getGovIDAsString()))
                {
                    newTable.Rows.Add(addStaffTableRow(staff));
                }
            }

            TableRow emptyRow = new TableRow();            

            TableCell newCell = new TableCell();
            newCell.Text = "&nbsp;";
            newCell.ColumnSpan = 4;
            emptyRow.Cells.Add(newCell);

            newTable.Rows.Add(emptyRow);


            return newTable;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            List<School> allSchools = null;
            List<StaffMember> allStaff = null;

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allSchools = School.loadAllSchools(connection);
                allStaff = StaffMember.loadAllStaff(connection);
            }

            panel_TablePanel.Controls.Clear();

            panel_TablePanel.Controls.Add(buildStaffTable(new School("No School", "0", "0", "", "default_logo.gif"), allStaff));

            foreach (School school in allSchools)
            {
                panel_TablePanel.Controls.Add(buildStaffTable(school, allStaff));
            }

            



        }
    }
}