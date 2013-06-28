using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Birthdays
{
    public partial class index : System.Web.UI.Page
    {

        protected TableRow addTableHeader()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

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

            TableCell cell_studentID = new TableCell();
            cell_studentID.Width = 125;
            cell_studentID.Text = "Student ID";
            newRow.Cells.Add(cell_studentID);

            TableCell cell_school = new TableCell();
            cell_school.Width = 250;
            cell_school.Text = "School";
            newRow.Cells.Add(cell_school);
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

            TableCell cell_age = new TableCell();
            cell_age.Width = 250;
            cell_age.Text = "Current Age";
            newRow.Cells.Add(cell_age);

            TableCell cell_homeroom = new TableCell();
            cell_homeroom.Width = 250;
            cell_homeroom.Text = "Homeroom";
            newRow.Cells.Add(cell_homeroom);

            return newRow;
        }
        protected TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "Row";

            TableCell cell_givenName = new TableCell();
            cell_givenName.Text =student.getGivenName();
            newRow.Cells.Add(cell_givenName);

            TableCell cell_middleName = new TableCell();
            cell_middleName.Text = student.getMiddleName();
            newRow.Cells.Add(cell_middleName);

            TableCell cell_surname = new TableCell();
            cell_surname.Text = student.getSN();
            newRow.Cells.Add(cell_surname);

            TableCell cell_studentID = new TableCell();
            cell_studentID.Text = student.getStudentID();
            newRow.Cells.Add(cell_studentID);

            TableCell cell_school = new TableCell();
            cell_school.Text = student.getSchoolName();
            newRow.Cells.Add(cell_school);

            TableCell cell_grade = new TableCell();
            cell_grade.Text = student.getGrade();
            newRow.Cells.Add(cell_grade);

            TableCell cell_gender = new TableCell();
            cell_gender.Text = student.getGender();
            newRow.Cells.Add(cell_gender);

            TableCell cell_dob = new TableCell();
            cell_dob.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(cell_dob);

            TableCell cell_age = new TableCell();
            cell_age.Text = student.getAge().ToString();
            newRow.Cells.Add(cell_age);

            TableCell cell_homeroom = new TableCell();
            cell_homeroom.Text = student.getHomeRoom();
            newRow.Cells.Add(cell_homeroom);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            List<Student> birthdayToday = new List<Student>();
            List<Student> birthdayTomorrow = new List<Student>();
            List<Student> birthdayThisMonth = new List<Student>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            foreach (Student student in allStudents)
            {
                if ((student.getDateOfBirth().Month == DateTime.Now.Month) && (student.getDateOfBirth().Day == DateTime.Now.Day))
                {
                    birthdayToday.Add(student);
                }

                if ((student.getDateOfBirth().Month == DateTime.Now.Month) && (student.getDateOfBirth().Day == DateTime.Now.AddDays(1).Day))
                {
                    birthdayTomorrow.Add(student);
                }

                if (student.getDateOfBirth().Month == DateTime.Now.Month)
                {
                    birthdayThisMonth.Add(student);
                }
            }


            tblToday.Rows.Clear();
            tblToday.Rows.Add(addTableHeader());
            foreach (Student student in birthdayToday)
            {
                tblToday.Rows.Add(addStudentRow(student));
            }

            tblTomorrow.Rows.Clear();
            tblTomorrow.Rows.Add(addTableHeader());
            foreach (Student student in birthdayTomorrow)
            {
                tblTomorrow.Rows.Add(addStudentRow(student));
            }

            tblthisMonth.Rows.Clear();
            tblthisMonth.Rows.Add(addTableHeader());
            foreach (Student student in birthdayThisMonth)
            {
                tblthisMonth.Rows.Add(addStudentRow(student));
            }

            lblMonthCount.Text = "(" + birthdayThisMonth.Count.ToString() + ")";
            lblTodayCount.Text = "(" + birthdayToday.Count.ToString() + ")";
            lblTomorrowCount.Text = "(" + birthdayTomorrow.Count.ToString() + ")";

        }
    }
}