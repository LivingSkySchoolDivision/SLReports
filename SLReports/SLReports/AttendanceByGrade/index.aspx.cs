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
                from_Month.Items.Add(newItem);

                newItem = new ListItem();
                newItem.Text = Months[month - 1];
                newItem.Value = month.ToString();
                to_Month.Items.Add(newItem);
            }

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
            
            /* Figure out a date range */
            DateTime Date_From = new DateTime(int.Parse(from_Year.SelectedValue), int.Parse(from_Month.SelectedValue), int.Parse(from_Day.SelectedValue));
            DateTime Date_To = new DateTime(int.Parse(to_Year.SelectedValue), int.Parse(to_Month.SelectedValue), int.Parse(to_Day.SelectedValue));

            /* Load the next page */
            Response.Redirect(@"/SLReports/AttendanceByGrade/GetPDF.aspx?schoolid=" + SelectedSchool.getGovIDAsString() + "&grade=" + selectedGrade + "&from_date=" + Date_From.ToString() + "&to_date=" + Date_To.ToString());



        }
    }
}