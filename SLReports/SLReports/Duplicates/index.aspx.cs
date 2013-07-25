using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Duplicates
{
    public partial class index : System.Web.UI.Page
    {
        private TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = student.getStudentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDisplayName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGovernmentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGender();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getSchoolName();
            newRow.Cells.Add(newCell);          


            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }


            Dictionary<string, Student> workspace_govid = new Dictionary<string, Student>();
            Dictionary<string, Student> workspace_names = new Dictionary<string, Student>();
            List<Student> foundDuplicates_govid = new List<Student>();
            List<Student> foundDuplicates_names = new List<Student>();

            foreach (Student student in allStudents)
            {
                if (!string.IsNullOrEmpty(student.getGovernmentID()))
                {
                    if (!workspace_govid.ContainsKey(student.getGovernmentID()))
                    {
                        workspace_govid.Add(student.getGovernmentID(), student);
                    }
                    else
                    {
                        foundDuplicates_govid.Add(student);
                        foundDuplicates_govid.Add(workspace_govid[student.getGovernmentID()]);
                    }
                }

                String condensedName = student.getDisplayName().ToLower().Replace(" ",string.Empty).Replace("-", string.Empty);
                if (!workspace_names.ContainsKey(condensedName))
                {
                    workspace_names.Add(condensedName, student);
                }
                else
                {
                    foundDuplicates_names.Add(student);
                    foundDuplicates_names.Add(workspace_names[condensedName]);
                }
            }

            foreach (Student student in foundDuplicates_govid)
            {
                tblGovIDs.Rows.Add(addStudentRow(student));
            }
            lblIdsCount.Text = "(" + foundDuplicates_govid.Count.ToString() + ")";

            foreach (Student student in foundDuplicates_names)
            {
                tblNames.Rows.Add(addStudentRow(student));
            }
            lblNamesCount.Text = "(" + foundDuplicates_names.Count.ToString() + ")";


        }
    }
}