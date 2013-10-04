using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.AttendanceByGrade
{
    public partial class index : System.Web.UI.Page
    {
        List<School> AllSchools;
        List<Student> SchoolStudents;
        School SelectedSchool;
        String selectedGrade;

        protected void Page_Init(object sender, EventArgs e)
        { 
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Load all schools */
                AllSchools = School.loadAllSchools(connection);
                AllSchools.Sort();
            }

            foreach (School school in AllSchools)
            {
                ListItem newItem = new ListItem();
                newItem.Text = school.getName();
                newItem.Value = school.getGovIDAsString();
                drpSchoolList.Items.Add(newItem);
            }
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            /* Get all grades for selected school and list them */
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedSchool = School.loadThisSchool(connection, int.Parse(drpSchoolList.SelectedValue));

                /* Load all students for this school */
                SchoolStudents = Student.loadStudentsFromThisSchool(connection, int.Parse(SelectedSchool.getGovIDAsString()));
            }

            List<String> availableGrades = new List<String>();

            if (SchoolStudents != null)
            {
                // Determine all possible grades
                foreach (Student thisStudent in SchoolStudents)
                {
                    if (!availableGrades.Contains(thisStudent.getGrade()))
                    {
                        availableGrades.Add(thisStudent.getGrade());
                    }
                    
                }

                availableGrades.Sort();

                /* Display the grades in a listbox */
                drpGradeList.Items.Clear();
                foreach (String thisGrade in availableGrades)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = thisGrade;
                    newItem.Value = thisGrade;
                    drpGradeList.Items.Add(newItem);
                }
                /* Make the listbox unhidden */
                TableRow_Grade.Visible = true;
                TableRow_Date.Visible = false;
            }            

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedSchool = School.loadThisSchool(connection, int.Parse(drpSchoolList.SelectedValue));

                /* Load all students for this school */
                SchoolStudents = Student.loadStudentsFromThisSchool(connection, int.Parse(SelectedSchool.getGovIDAsString()));
            }

            /* Get the selected grade */
            selectedGrade = drpGradeList.SelectedValue;



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

        protected void Button3_Click(object sender, EventArgs e)
        {
            /* Not sure why I need to reload this every time... */

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedSchool = School.loadThisSchool(connection, int.Parse(drpSchoolList.SelectedValue));

                /* Load all students for this school */
                SchoolStudents = Student.loadStudentsFromThisSchool(connection, int.Parse(SelectedSchool.getGovIDAsString()));
            }

            selectedGrade = drpGradeList.SelectedValue;
                       
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
                Response.Redirect(@"/SLReports/AttendanceByGrade/GetPDF.aspx?schoolid=" + SelectedSchool.getGovIDAsString() + "&grade=" + selectedGrade + "&from_date=" + startDate.ToString() + "&to_date=" + endDate.ToString());

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