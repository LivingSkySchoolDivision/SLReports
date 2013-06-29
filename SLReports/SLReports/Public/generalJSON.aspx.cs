using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Public
{
    public partial class generalJSON : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            List<School> allSchools = new List<School>();
            List<StaffMember> allStaff = new List<StaffMember>();

            float numStudents = allStudents.Count;
            float numMales = 0;
            float numFemales = 0;
            float numBirthdaysToday = 0;
            float numBirthdaysTomorrow = 0;
            float numBirthadysThisMonth = 0;

            List<String> allCities = new List<String>();
            List<String> allRegions = new List<String>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
                allSchools = School.loadAllSchools(connection);
                allStaff = StaffMember.loadAllStaff(connection);
            }

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
                    numBirthdaysToday++;
                }

                if (
                    (student.getDateOfBirth().Month == DateTime.Now.Month) &&
                    (student.getDateOfBirth().Day == DateTime.Now.AddDays(1).Day)
                    )
                {
                    numBirthdaysTomorrow++;
                }

                if (student.getDateOfBirth().Month == DateTime.Now.Month)
                {
                    numBirthadysThisMonth++;
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

            /* Generate XML */
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{");

            Response.Write("\"Schools\": {");
            Response.Write("\"SchoolCount\": " + allSchools.Count + ",");
            Response.Write("\"RegionCount\": " + allRegions.Count + "");
            Response.Write("},");

            Response.Write("\"Students\": {");
            Response.Write("\"Total\": " + allStudents.Count + ",");
            Response.Write("\"Male\": " + numMales + ",");
            Response.Write("\"Female\": " + numFemales + ",");

            Response.Write("\"BirthdaysToday\": " + numBirthdaysToday + ",");
            Response.Write("\"BirthdaysTomorrow\": " + numBirthdaysTomorrow + ",");
            Response.Write("\"BirthdaysThisMonth\": " + numBirthadysThisMonth + "");

            Response.Write("},");

            Response.Write("\"Staff\": {");
            Response.Write("\"Total\": " + allStaff.Count + "");
            Response.Write("}");


            Response.Write("}");
            Response.End();

        }
    }
}