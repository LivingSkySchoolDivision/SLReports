using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.JSON
{
    public partial class getEnrollmentStats : System.Web.UI.Page
    {
        String dbConnectionString = LSKYCommon.dbConnectionString_SchoolLogic;
        String dbConnectionString_Local = LSKYCommon.dbConnectionString_DataExplorer;

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allStudents = Student.loadAllStudents(connection);
            }


            Dictionary<string, int> enrollment_Communities = new Dictionary<string, int>();
            Dictionary<string, int> enrollment_Schools = new Dictionary<string, int>();

            int maleCount = 0;
            int femaleCount = 0;
            int birthdaysToday = 0;

            foreach (Student student in allStudents)
            {

                // Get male / female counts
                if (student.getGender().ToLower() == "male")
                {
                    maleCount++;
                }
                else
                {
                    femaleCount++;
                }

                // Get birthays today
                if ((student.getDateOfBirth().Month == DateTime.Today.Month) && (student.getDateOfBirth().Day == DateTime.Today.Day)) 
                {
                    birthdaysToday++;
                }
                
                // Get enrollment counts by community
                if (!enrollment_Communities.ContainsKey(student.getCity()))
                {
                    enrollment_Communities.Add(student.getCity(), 0);
                }
                enrollment_Communities[student.getCity()]++;

                // Get enrollment counts by school
                if (!enrollment_Schools.ContainsKey(student.getSchoolName()))
                {
                    enrollment_Schools.Add(student.getSchoolName(), 0);
                }
                enrollment_Schools[student.getSchoolName()]++;

            }

            // I'd rather calculate this here than in javascript, because I don't really care for javascript
            double malePercent = Math.Round(((double)maleCount / (double)allStudents.Count) * 100, 0);
            double femalePercent = Math.Round(((double)femaleCount / (double)allStudents.Count) * 100, 0);
            
            // Sort the community and school lists
            List<KeyValuePair<string, int>> communities = enrollment_Communities.ToList();
            List<KeyValuePair<string, int>> schools = enrollment_Schools.ToList();

            communities.Sort(
                delegate(KeyValuePair<string, int> firstPair,
                KeyValuePair<string, int> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
                );

            schools.Sort(
                delegate(KeyValuePair<string, int> firstPair,
                KeyValuePair<string, int> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
                );

            communities.Reverse();
            schools.Reverse();

            // Output JSON file


            //Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Students\": {\n");

            Response.Write("\"TotalEnrolled\": \"" + allStudents.Count + "\",\n");
            Response.Write("\"Male\": " + maleCount + ",\n");
            Response.Write("\"Female\": " + femaleCount + ",\n");
            Response.Write("\"MalePercent\": " + malePercent + ",\n");
            Response.Write("\"FemalePercent\": " + femalePercent + ",\n");
            Response.Write("\"BirthdaysToday\": " + birthdaysToday + ",\n");
            
            Response.Write("\"Communities\": [\n");

            int itemCounter = 0;
            foreach (KeyValuePair<string, int> kvp in communities)
            {
                Response.Write(" {\"name\": \"" + kvp.Key + "\",");
                Response.Write("\"count\": " + kvp.Value + "} ");

                itemCounter++;
                if (itemCounter < communities.Count)
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("],\n");
            
            Response.Write("\"Schools\": [\n");

            itemCounter = 0;
            foreach (KeyValuePair<string, int> kvp in schools)
            {
                Response.Write(" {\"name\": \"" + kvp.Key + "\",");
                Response.Write("\"count\": " + kvp.Value + "} ");

                itemCounter++;
                if (itemCounter < schools.Count)
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("]\n");

            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();


        }
    }
}