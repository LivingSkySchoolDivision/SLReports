using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentDiscrepancy
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Add schools to the dropdown list
            if (!IsPostBack)
            {
                List<School> allSchools;
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Value = school.getGovID().ToString();
                    newItem.Text = school.getName();
                    drpSchool.Items.Add(newItem);
                }
            }
        }

        private TableRow addIDRow(int idnumber)
        {
            TableRow newRow = new TableRow();
            TableCell newCell = new TableCell();
            newCell.Text = idnumber.ToString();
            newRow.Cells.Add(newCell);
            return newRow;
        }

        private TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();
            TableCell newCell = new TableCell();
            newCell.Text = student.getStudentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDisplayName();
            newRow.Cells.Add(newCell);

            return newRow;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string[] input = txtInput.Text.Split('\n');

            List<int> inputtedIDNumbers = new List<int>();

            foreach (string segment in input)
            {
                if (!string.IsNullOrEmpty(segment))
                {
                    inputtedIDNumbers.Add(int.Parse(segment));
                }
            }

            lblStatus.Text = "Found " + inputtedIDNumbers.Count + " valid id numbers.";

            // Parse the selected school
            int selectedSchoolID = 0;
            if (!int.TryParse(drpSchool.SelectedValue, out selectedSchoolID))
            {
                selectedSchoolID = 0;
            }

            // Load all students
            List<Student> allStudents = new List<Student>(); 
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                if (selectedSchoolID == 0)
                {
                    allStudents = Student.loadAllStudents(connection);
                }
                else
                {
                    allStudents = Student.loadStudentsFromThisSchool(connection, selectedSchoolID);
                }
            }

            List<int> idNumbersMissingFromDatabase = new List<int>();
            List<Student> studentsMissingFromInput = new List<Student>();
            List<Student> studentsFoundInDatabase = new List<Student>();
            
            foreach (Student student in allStudents)
            {
                bool foundMatch = false;
                foreach (int testForThisID in inputtedIDNumbers)
                {
                    if (testForThisID == int.Parse(student.getStudentID()))
                    {
                        foundMatch = true;
                    }
                }

                if (foundMatch)
                {
                    studentsFoundInDatabase.Add(student);
                }
                else
                {
                    studentsMissingFromInput.Add(student);
                }
            }

            foreach (int testForThisID in inputtedIDNumbers)
            {
                bool foundMatch = false;
                foreach (Student student in allStudents)
                {
                    if (testForThisID == int.Parse(student.getStudentID()))
                    {
                        foundMatch = true;
                    }
                }

                if (!foundMatch)
                {
                    idNumbersMissingFromDatabase.Add(testForThisID);
                }
            }

            foreach (int id in idNumbersMissingFromDatabase)
            {
                tblMissingFromDataSite.Rows.Add(addIDRow(id));
            }

            foreach (Student student in studentsMissingFromInput)
            {
                tblMissingFromInput.Rows.Add(addStudentRow(student));   
            }

            foreach (Student student in studentsFoundInDatabase)
            {
                tblMatchingStudents.Rows.Add(addStudentRow(student));
            }

            lblMissingFromDatabaseCount.Text = "(" + idNumbersMissingFromDatabase.Count + ")";
            lblMissingFromInputCount.Text = "(" + studentsMissingFromInput.Count + ")";
            lblPresentCount.Text = "(" + studentsFoundInDatabase.Count + ")";

            if (
                (idNumbersMissingFromDatabase.Count > 0) ||
                (studentsMissingFromInput.Count > 0) ||
                (studentsFoundInDatabase.Count > 0)
                )
            {
                tblResults.Visible = true;
            }
            else
            {
                tblResults.Visible = false;
            }


        }
    }
}