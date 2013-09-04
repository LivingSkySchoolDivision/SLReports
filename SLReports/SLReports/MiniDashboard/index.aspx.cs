using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.MiniDashboard
{
    public partial class index : System.Web.UI.Page
    {
        String dbConnectionString = LSKYCommon.dbConnectionString_SchoolLogic;
        String dbConnectionString_Local = LSKYCommon.dbConnectionString_DataExplorer;

        protected void Page_Load(object sender, EventArgs e)
        {

            // Load data for the statistics box
            List<Student> allStudents = new List<Student>();
            List<School> allSchools = new List<School>();
            List<StaffMember> allStaff = new List<StaffMember>();
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
                allSchools = School.loadAllSchools(connection);
                allStaff = StaffMember.loadAllStaff(connection);
            }

            float numStudents = allStudents.Count;
            float numMales = 0;
            float numFemales = 0;
            float numBirthdays = 0;

            List<String> allCities = new List<String>();
            List<String> allRegions = new List<String>();

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

                if (!allRegions.Contains(student.getRegion()))
                {
                    allRegions.Add(student.getRegion());
                }

            }

            lblActiveStudentCount.Text = numStudents.ToString();
            lblSchoolCount.Text = allSchools.Count.ToString();
            lblStaffCount.Text = allStaff.Count.ToString();

            lblMaleCount.Text = numMales.ToString();
            lblFemaleCount.Text = numFemales.ToString();
            lblBirthdayCount.Text = numBirthdays.ToString();


            lblMalePercent.Text = Math.Round((float)((numMales / numStudents) * 100)) + "%";
            lblFemalePercent.Text = Math.Round((float)((numFemales / numStudents) * 100)) + "%";

            lblCities.Text = allCities.Count().ToString();
            lblRegions.Text = allRegions.Count().ToString();

        }
    }
}