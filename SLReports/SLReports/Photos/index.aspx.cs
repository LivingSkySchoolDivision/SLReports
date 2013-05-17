using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Photos
{
    public partial class index : System.Web.UI.Page
    {
        List<School> AllSchools;
        List<Student> AllStudents;
        List<Student> StudentsWithPhoto;
        List<Student> StudentsWithoutPhoto;

        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {                
                AllSchools = School.loadAllSchools(connection);
            }

            if (!IsPostBack)
            {
                foreach (School school in AllSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = school.getName();
                    newItem.Value = school.getGovID();
                    drpSchoolList.Items.Add(newItem);
                }
            }



        }

        protected TableRow addStudentRow(Student s)
        {
            TableRow newTR = new TableRow();
            newTR.CssClass = "row";

            TableCell newTD_StudentNumber = new TableCell();
            newTD_StudentNumber.Text = s.getStudentID();

            TableCell newTD_GivenName = new TableCell();
            newTD_GivenName.Text = s.getGivenName();

            TableCell newTD_Surname = new TableCell();
            newTD_Surname.Text = s.getSN();

            TableCell newTD_Grade = new TableCell();
            newTD_Grade.Text = s.getGrade();

                       

            newTR.Cells.Add(newTD_StudentNumber);
            newTR.Cells.Add(newTD_GivenName);
            newTR.Cells.Add(newTD_Surname);
            newTR.Cells.Add(newTD_Grade);

            TableCell newTD_PhotoLink = new TableCell();
            if (s.hasPhoto())
            {
                /* jkpopimage('food1.jpg', 325, 445, 'Breakfast is served.'); return false*/
                //newTD_PhotoLink.Text = "<a onMouseOut=\"document.photo_preview.src='#'\" onMouseOver=\"document.photo_preview.src='/SLReports/Photos/GetPhoto.aspx?studentnumber=" + s.getStudentID() + "'\" href=\"#\">Mouse Over</a>";
                newTD_PhotoLink.Text = "<a onClick=\"jkpopimage('/SLReports/Photos/GetPhoto.aspx?studentnumber=" + s.getStudentID() + "', 350, 450, '" + s.getDisplayName() + " ("+s.getStudentID()+")'); return false;\" href=\"#\">Click</a>";
                newTD_PhotoLink.CssClass = "noPrint";
                newTR.Cells.Add(newTD_PhotoLink);
            } 

            return newTR;

        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                int schoolID = -1;
                try {
                    schoolID = int.Parse(drpSchoolList.SelectedValue);
                }
                catch {}

                AllStudents = Student.loadStudentsFromThisSchool(connection, schoolID);
            }

            StudentsWithPhoto = new List<Student>();
            StudentsWithoutPhoto = new List<Student>();

            foreach (Student s in AllStudents)
            {
                if (s.hasPhoto())
                {
                    StudentsWithPhoto.Add(s);
                }
                else
                {
                    StudentsWithoutPhoto.Add(s);
                }
            }

            if (AllStudents != null)
            {
                tblContainer.Visible = true;
                lblTotalWithout.Text = "Missing photo: " + StudentsWithoutPhoto.Count + " / " + AllStudents.Count;
                lblTotalWith.Text = "Has photo: " + StudentsWithPhoto.Count + " / " + AllStudents.Count;

                foreach (Student s in StudentsWithoutPhoto)
                {
                    tblWithoutPhoto.Rows.Add(addStudentRow(s));
                } 
                
                foreach (Student s in StudentsWithPhoto)
                {
                    tblWithPhoto.Rows.Add(addStudentRow(s));
                }
            }
            else
            {
                tblContainer.Visible = false;
            }
        }
    }
}