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
    public partial class generalXML : System.Web.UI.Page
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
            Response.ContentType = "text/xml; charset=utf-8";
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            Response.Write("<SchoolLogicStats>");

            Response.Write("<Schools>");
            Response.Write("<SchoolCount>" + allSchools.Count + "</SchoolCount>");
            Response.Write("<RegionCount>" + allRegions.Count + "</RegionCount>");
            Response.Write("</Schools>");

            Response.Write("<Students>");
            Response.Write("<Total>" + allStudents.Count + "</Total>");
            Response.Write("<Male>" + numMales + "</Male>");
            Response.Write("<Female>" + numFemales +"</Female>");
            Response.Write("<Birthdays>");
            Response.Write("<Today>" + numBirthdaysToday + "</Today>");
            Response.Write("<Tomorrow>" + numBirthdaysTomorrow + "</Tomorrow>");
            Response.Write("<ThisMonth>" + numBirthadysThisMonth + "</ThisMonth>");
            Response.Write("</Birthdays>");
            Response.Write("</Students>");

            Response.Write("<Staff>");
            Response.Write("<Total>" + allStaff.Count + "</Total>");
            Response.Write("</Staff>");


            Response.Write("</SchoolLogicStats>");
            Response.End();

        }
    }
}