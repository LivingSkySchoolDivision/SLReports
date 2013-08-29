using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.FindStudent
{
    public partial class index : System.Web.UI.Page
    {
        private String highLightSeachString(String input, String searchString)
        {
            //return input.Replace(searchString, "<span style=\"background-color: yellow;\">" + searchString + "</span>");
            return Regex.Replace(input, searchString, "<span class=\"search_result\">" + searchString + "</span>", RegexOptions.IgnoreCase);
        }

        protected TableRow addTablePreHeader(int results)
        {
            TableRow newRow = new TableRow();

            TableCell newcell = new TableCell();            
            newcell.ColumnSpan = 9;
            newcell.Width = 125;
            newcell.Text = "Results: " + results;
            newcell.BorderWidth = 0;
            newRow.Cells.Add(newcell);

            return newRow;
        }

        protected TableRow addTableHeader()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell cell_studentID = new TableCell();
            cell_studentID.Width = 125;
            cell_studentID.Text = "Student ID";
            newRow.Cells.Add(cell_studentID);

            TableCell cell_givenName = new TableCell();
            cell_givenName.Width = 125;
            cell_givenName.Text = "Given Name";
            newRow.Cells.Add(cell_givenName);

            TableCell cell_middleName = new TableCell();
            cell_middleName.Width = 125;
            cell_middleName.Text = "Middle Name";
            newRow.Cells.Add(cell_middleName);

            TableCell cell_surname = new TableCell();
            cell_surname.Width = 125;
            cell_surname.Text = "Surname";
            newRow.Cells.Add(cell_surname);

            TableCell cell_school = new TableCell();
            cell_school.Width = 250;
            cell_school.Text = "School";
            newRow.Cells.Add(cell_school);

            TableCell cell_govID = new TableCell();
            cell_govID.Width = 125;
            cell_govID.Text = "Gov ID";
            newRow.Cells.Add(cell_govID);
            
            TableCell cell_statusNo = new TableCell();
            cell_govID.Width = 125;
            cell_govID.Text = "Status #";
            newRow.Cells.Add(cell_statusNo);
            
            TableCell cell_grade = new TableCell();
            cell_grade.Width = 50;
            cell_grade.Text = "Grade";
            newRow.Cells.Add(cell_grade);

            TableCell cell_gender = new TableCell();
            cell_gender.Width = 50;
            cell_gender.Text = "Gender";
            newRow.Cells.Add(cell_gender);

            TableCell cell_dob = new TableCell();
            cell_dob.Width = 125;
            cell_dob.Text = "Date of Birth";
            newRow.Cells.Add(cell_dob);

            TableCell cell_homeroom = new TableCell();
            cell_homeroom.Width = 250;
            cell_homeroom.Text = "Homeroom";
            newRow.Cells.Add(cell_homeroom);

            TableCell cell_ldap = new TableCell();
            cell_ldap.Width = 250;
            cell_ldap.Text = "LDAP Username";
            newRow.Cells.Add(cell_ldap);

            return newRow;
        }

        protected TableRow addStudentRow(Student student, String searchString)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "Row";

            TableCell cell_studentID = new TableCell();
            cell_studentID.Text = highLightSeachString(student.getStudentID(), searchString);
            newRow.Cells.Add(cell_studentID);

            TableCell cell_givenName = new TableCell();
            cell_givenName.Text = highLightSeachString(student.getFirstName(), searchString);
            newRow.Cells.Add(cell_givenName);

            TableCell cell_middleName = new TableCell();
            cell_middleName.Text = highLightSeachString(student.getLegalMiddleName(), searchString);
            newRow.Cells.Add(cell_middleName);

            TableCell cell_surname = new TableCell();
            cell_surname.Text = highLightSeachString(student.getLastName(), searchString);
            newRow.Cells.Add(cell_surname);
            
            TableCell cell_school = new TableCell();
            cell_school.Text = student.getSchoolName();
            newRow.Cells.Add(cell_school);

            TableCell cell_govID = new TableCell();
            cell_govID.Text = highLightSeachString(student.getGovernmentID(), searchString);
            newRow.Cells.Add(cell_govID);
            
            TableCell cell_statusNo = new TableCell();
            cell_govID.Text = highLightSeachString(student.getStatusNo(), searchString);
            newRow.Cells.Add(cell_statusNo);           
             
            TableCell cell_grade = new TableCell();
            cell_grade.Text = highLightSeachString(student.getGrade(), searchString);
            newRow.Cells.Add(cell_grade);

            TableCell cell_gender = new TableCell();
            cell_gender.Text = student.getGender();
            newRow.Cells.Add(cell_gender);

            TableCell cell_dob = new TableCell();
            cell_dob.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(cell_dob);

            TableCell cell_homeroom = new TableCell();
            cell_homeroom.Text = student.getHomeRoom();
            newRow.Cells.Add(cell_homeroom);

            TableCell cell_ldap = new TableCell();
            cell_ldap.Text = highLightSeachString(student.LDAPUserName, searchString);
            newRow.Cells.Add(cell_ldap);

            return newRow;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>(); 
            List<Student> searchResults = new List<Student>();

            String searchQuery = txtSearch.Text;

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            foreach (Student student in allStudents)
            {
                if (
                    (student.getStudentID().ToLower().Contains(searchQuery.ToLower())) ||
                    (student.getFirstName().ToLower().Contains(searchQuery.ToLower())) ||
                    (student.getLastName().ToLower().Contains(searchQuery.ToLower())) ||
                    (student.getGovernmentID().ToLower().Contains(searchQuery.ToLower())) ||
                    (student.getStatusNo().ToLower().Contains(searchQuery.ToLower())) ||
                    (student.LDAPUserName.ToLower().Contains(searchQuery.ToLower())) ||
                    (student.getLegalMiddleName().ToLower().Contains(searchQuery.ToLower()))
                    )
                {
                    searchResults.Add(student);
                }
            }

            tblResults.Rows.Clear();
            tblResults.Rows.Add(addTablePreHeader(searchResults.Count));
            tblResults.Rows.Add(addTableHeader());
            foreach (Student student in searchResults)
            {
                tblResults.Rows.Add(addStudentRow(student, searchQuery));
            }
        }
    }
}