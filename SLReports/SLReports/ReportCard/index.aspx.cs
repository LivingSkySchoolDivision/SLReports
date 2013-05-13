using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class index : System.Web.UI.Page
    {
        Student SelectedStudent;
        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load the current date into the date label field */
            lblDate.Text = DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToShortTimeString();

            /* Load a specific student, for testing */
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedStudent = Student.loadThisStudent(connection, "12511");
            }

            if (SelectedStudent != null)
            {
                lblStudentName.Text = SelectedStudent.getDisplayName();
                lblHomeRoom.Text = SelectedStudent.getHomeRoom();
            }
        }
    }
}