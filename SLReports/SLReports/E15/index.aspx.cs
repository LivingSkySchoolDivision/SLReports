using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.PotentialProblems
{
    public partial class index : System.Web.UI.Page
    {
        List<Student> AllStudents;
        List<Student> DisplayedStudents;

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbUser = @"sql_readonly";
            String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
            String dbHost = "dcsql.lskysd.ca";
            String dbDatabase = "SchoolLogicDB";
            //String dbDatabase = "SchoolLogicDB";
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                AllStudents = Student.loadAllStudents(connection);
            }

            DisplayedStudents = new List<Student>();
            foreach (Student s in AllStudents)
            {

                if (s.getInStatusCode().ToLower().Equals("e15"))
                {
                    DisplayedStudents.Add(s);
                }
                
            }

            lblTotal.Text = "Total: " + DisplayedStudents.Count;

            foreach (Student s in DisplayedStudents)
            {   
                TableRow newTR = new TableRow();
                newTR.CssClass = "row";

                TableCell newTD_School = new TableCell();
                newTD_School.Text = s.getSchoolName();

                TableCell newTD_StudentNumber = new TableCell();
                newTD_StudentNumber.Text = s.getStudentID();

                TableCell newTD_GivenName = new TableCell();
                newTD_GivenName.Text = s.getGivenName();
                    
                TableCell newTD_Surname = new TableCell();
                newTD_Surname.Text = s.getSN();

                TableCell newTD_Grade = new TableCell();
                newTD_Grade.Text = s.getGrade();
                
                TableCell newTD_InDate = new TableCell();
                newTD_InDate.Text = s.getEnrollDate().ToLongDateString();

                TableCell newTD_InStatusCode = new TableCell();
                newTD_InStatusCode.Text = s.getInStatusCode();

                TableCell newTD_InStatus = new TableCell();
                newTD_InStatus.Text = s.getInStatus();
                    
                newTR.Cells.Add(newTD_School);
                newTR.Cells.Add(newTD_Grade);
                newTR.Cells.Add(newTD_StudentNumber);
                newTR.Cells.Add(newTD_GivenName);
                newTR.Cells.Add(newTD_Surname);
                newTR.Cells.Add(newTD_InDate);
                newTR.Cells.Add(newTD_InStatusCode);
                newTR.Cells.Add(newTD_InStatus);

                tblE15Students.Rows.Add(newTR);

            }

        }
    }
}