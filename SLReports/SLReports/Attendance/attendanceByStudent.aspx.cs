using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Attendance
{
    public partial class attendanceByStudent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load schools
                List<School> allSchools = new List<School>();
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = school.getName();
                    newItem.Value = school.getGovIDAsString();
                    drpSchoolList.Items.Add(newItem);
                }
            }
        }

        protected void btnSchool_Click(object sender, EventArgs e)
        {
            // Load all students for the selected school
            int schoolID = 0;
            if (int.TryParse(drpSchoolList.SelectedValue, out schoolID))
            {
                List<Student> allStudents = new List<Student>();
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    allStudents = Student.loadStudentsFromThisSchool(connection, schoolID);
                }

                foreach (Student student in allStudents)
                {
                    drpStudentList.Items.Add(new ListItem(student.getLastName() + ", " + student.getFirstName() + " (" + student.getStudentID() + ")", student.getStudentID()));
                }

                if (allStudents.Count > 0)
                {
                    TableRow_Students.Visible = true;
                }
            }
        }

        protected void btnStudent_Click(object sender, EventArgs e)
        {
            /* Show the date picker */
            from_Day.Items.Clear();
            from_Month.Items.Clear();
            from_Year.Items.Clear();
            to_Day.Items.Clear();
            to_Month.Items.Clear();
            to_Year.Items.Clear();

            for (int year = (DateTime.Now.Year - 5); year < (DateTime.Now.Year + 5); year++)
            {
                ListItem newItem = null;
                newItem = new ListItem();
                newItem.Text = year.ToString();
                newItem.Value = year.ToString();
                if (year == DateTime.Now.Year)
                {
                    newItem.Selected = true;
                }
                from_Year.Items.Add(newItem);


                newItem = new ListItem();
                newItem.Text = year.ToString();
                newItem.Value = year.ToString();
                if (year == DateTime.Now.Year)
                {
                    newItem.Selected = true;
                }
                to_Year.Items.Add(newItem);
            }

            String[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            for (int month = 1; month <= 12; month++)
            {
                ListItem newItem = null;
                newItem = new ListItem();
                newItem.Text = Months[month - 1];
                newItem.Value = month.ToString();
                if (month == DateTime.Now.Month)
                {
                    newItem.Selected = true;
                }
                from_Month.Items.Add(newItem);

                newItem = new ListItem();
                newItem.Text = Months[month - 1];
                newItem.Value = month.ToString();
                if (month == DateTime.Now.Month)
                {
                    newItem.Selected = true;
                }
                to_Month.Items.Add(newItem);
            }


            ListItem firstDay_From = new ListItem("First Day", "1");
            if (!IsPostBack)
            {
                firstDay_From.Selected = true;
            }
            from_Day.Items.Add(firstDay_From);

            ListItem lastDay_To = new ListItem("Last Day", "31");
            if (!IsPostBack)
            {
                lastDay_To.Selected = true;
            }
            to_Day.Items.Add(lastDay_To);

            for (int day = 1; day <= 31; day++)
            {
                ListItem newItem = null;
                newItem = new ListItem();
                newItem.Text = day.ToString();
                newItem.Value = day.ToString();
                to_Day.Items.Add(newItem);

                newItem = new ListItem();
                newItem.Text = day.ToString();
                newItem.Value = day.ToString();

                from_Day.Items.Add(newItem);
            }

            TableRow_Date.Visible = true;

        }

        protected void btnDate_Click(object sender, EventArgs e)
        {
            Student SelectedStudent = null;
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
            }

            if (SelectedStudent != null)
            {
                // Parse from date
                int startYear = -1;
                int startMonth = -1;
                int startDay = -1;

                int.TryParse(from_Year.SelectedValue, out startYear);
                int.TryParse(from_Month.SelectedValue, out startMonth);
                int.TryParse(from_Day.SelectedValue, out startDay);

                // Parse to date
                int endYear = -1;
                int endMonth = -1;
                int endDay = -1;

                int.TryParse(to_Year.SelectedValue, out endYear);
                int.TryParse(to_Month.SelectedValue, out endMonth);
                int.TryParse(to_Day.SelectedValue, out endDay);

                if (
                    !(
                    (startYear == -1) ||
                    (startMonth == -1) ||
                    (startDay == -1) ||
                    (endYear == -1) ||
                    (endMonth == -1) ||
                    (endDay == -1)
                    )
                    )
                {

                    if (startDay > DateTime.DaysInMonth(startYear, startMonth))
                        startDay = DateTime.DaysInMonth(startYear, startMonth);

                    if (endDay > DateTime.DaysInMonth(endYear, endMonth))
                        endDay = DateTime.DaysInMonth(endYear, endMonth);

                    DateTime startDate = new DateTime(startYear, startMonth, startDay);
                    DateTime endDate = new DateTime(endYear, endMonth, endDay);

                    /* Load the next page */
                    Response.Redirect(@"attendanceByStudentPDF.aspx?studentid=" + SelectedStudent.getStudentID() + "&from_date=" + startDate.ToString() + "&to_date=" + endDate.ToString());

                }
                else
                {
                    Response.Write("Invalid date specified<br>");
                    Response.Write(" From Year: " + startYear + "<br>");
                    Response.Write(" From Month: " + startMonth + "<br>");
                    Response.Write(" From Day: " + startDay + "<br>");
                    Response.Write(" To Year: " + endYear + "<br>");
                    Response.Write(" To Month: " + endMonth + "<br>");
                    Response.Write(" To Day: " + endDay + "<br>");
                }
            }
        }

        protected void btnHTML_Click(object sender, EventArgs e)
        {
            Student SelectedStudent = null;
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
            }

            if (SelectedStudent != null)
            {
                // Parse from date
                int startYear = -1;
                int startMonth = -1;
                int startDay = -1;

                int.TryParse(from_Year.SelectedValue, out startYear);
                int.TryParse(from_Month.SelectedValue, out startMonth);
                int.TryParse(from_Day.SelectedValue, out startDay);

                // Parse to date
                int endYear = -1;
                int endMonth = -1;
                int endDay = -1;

                int.TryParse(to_Year.SelectedValue, out endYear);
                int.TryParse(to_Month.SelectedValue, out endMonth);
                int.TryParse(to_Day.SelectedValue, out endDay);

                if (
                    !(
                    (startYear == -1) ||
                    (startMonth == -1) ||
                    (startDay == -1) ||
                    (endYear == -1) ||
                    (endMonth == -1) ||
                    (endDay == -1)
                    )
                    )
                {

                    if (startDay > DateTime.DaysInMonth(startYear, startMonth))
                        startDay = DateTime.DaysInMonth(startYear, startMonth);

                    if (endDay > DateTime.DaysInMonth(endYear, endMonth))
                        endDay = DateTime.DaysInMonth(endYear, endMonth);

                    DateTime startDate = new DateTime(startYear, startMonth, startDay);
                    DateTime endDate = new DateTime(endYear, endMonth, endDay);

                    /* Load the next page */
                    Response.Redirect(@"getAttendance.aspx?studentid=" + SelectedStudent.getStudentID() + "&from_year=" + startDate.Year + "&from_month=" + startDate.Month+ "&from_day=" + startDate.Day + "&to_year=" + endDate.Year + "&to_month=" + endDate.Month + "&to_day=" + endDate.Day);

                }
                else
                {
                    Response.Write("Invalid date specified<br>");
                    Response.Write(" From Year: " + startYear + "<br>");
                    Response.Write(" From Month: " + startMonth + "<br>");
                    Response.Write(" From Day: " + startDay + "<br>");
                    Response.Write(" To Year: " + endYear + "<br>");
                    Response.Write(" To Month: " + endMonth + "<br>");
                    Response.Write(" To Day: " + endDay + "<br>");
                }
            }

        }
    }
}