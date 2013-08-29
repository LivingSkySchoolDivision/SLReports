using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentNames
{
    public partial class index : System.Web.UI.Page
    {
        private TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();

            bool colorLastNames = false;
            bool colorFirstNames = false;

            if (!student.displayFirstName.Equals(student.legalFirstName))
            {
                colorFirstNames = true;
            }

            if (!student.displayLastName.Equals(student.legalLastName))
            {
                colorLastNames = true;
            }


            TableCell cell_ID = new TableCell();
            cell_ID.Text = student.getStudentID();
            newRow.Cells.Add(cell_ID);

            TableCell cell_legalFirstName = new TableCell();
            cell_legalFirstName.Text = student.legalFirstName;
            if (colorFirstNames)
            {
                cell_legalFirstName.BorderColor = Color.Red;
                cell_legalFirstName.BorderWidth = 1;
            }
            newRow.Cells.Add(cell_legalFirstName);

            TableCell cell_displayFirstName = new TableCell();
            cell_displayFirstName.Text = student.displayFirstName;
            if (colorFirstNames)
            {
                cell_displayFirstName.BorderColor = Color.Red;
                cell_displayFirstName.BorderWidth = 1;
            }
            newRow.Cells.Add(cell_displayFirstName);

            TableCell cell_legalMiddleName = new TableCell();
            cell_legalMiddleName.Text = student.legalMiddleName;
            newRow.Cells.Add(cell_legalMiddleName);

            TableCell cell_legalLastName = new TableCell();
            cell_legalLastName.Text = student.legalLastName;
            if (colorLastNames)
            {
                cell_legalLastName.BorderColor = Color.Red;
                cell_legalLastName.BorderWidth = 1;
            }
            newRow.Cells.Add(cell_legalLastName);

            TableCell cell_displayLastName = new TableCell();
            cell_displayLastName.Text = student.displayLastName;
            if (colorLastNames)
            {
                cell_displayLastName.BorderColor = Color.Red;
                cell_displayLastName.BorderWidth = 1;
            }
            newRow.Cells.Add(cell_displayLastName);

            TableCell cell_SchoolName = new TableCell();
            cell_SchoolName.Text = student.getSchoolName();
            newRow.Cells.Add(cell_SchoolName);

            return newRow;    
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            List<Student> displayedStudents = new List<Student>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            foreach (Student student in allStudents)
            {
                // Check names
                if (
                    (!student.legalFirstName.Equals(student.displayFirstName)) ||
                    (!student.legalLastName.Equals(student.displayLastName))
                    )
                {
                    displayedStudents.Add(student);
                }

            }

            lblCount.Text = "Found " + displayedStudents.Count + " students";

            foreach (Student student in displayedStudents)
            {
                tblStudents.Rows.Add(addStudentRow(student));
            }
            
        }
    }
}