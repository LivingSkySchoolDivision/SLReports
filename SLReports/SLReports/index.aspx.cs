using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class index : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            List<School> allSchools = new List<School>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
                allSchools = School.loadAllSchools(connection);
            }

            float numStudents = allStudents.Count;
            float numMales = 0;
            float numFemales = 0;
            float numBirthdays = 0;

            List<String> allCities = new List<String>();

            foreach (Student student in allStudents)
            {
                if (student.getGender().ToLower() == "male")
                {
                    numMales++;
                }

                if (student.getGender().ToLower() == "female")
                {
                    numFemales++;
                }

                if (
                    (student.getDateOfBirth().Day == DateTime.Now.Day) &&
                    (student.getDateOfBirth().Month == DateTime.Now.Month)
                    )
                {
                    numBirthdays++;
                }

                if (!allCities.Contains(student.getCity()))
                {
                    allCities.Add(student.getCity());
                }
            }

            lblActiveStudentCount.Text = numStudents.ToString();
            lblSchoolCount.Text = allSchools.Count.ToString();

            lblMaleCount.Text = numMales.ToString() + " (" + Math.Round((float)((numMales / numStudents) * 100)) + "%)";
            lblFemaleCount.Text = numFemales.ToString() + " (" + Math.Round((float)((numFemales / numStudents) * 100)) + "%)";
            lblBirthdayCount.Text = numBirthdays.ToString();

            lblCities.Text = allCities.Count().ToString();

        }


    }
}