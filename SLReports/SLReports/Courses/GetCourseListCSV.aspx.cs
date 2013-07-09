using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Courses
{
    public partial class GetCourseListCSV : System.Web.UI.Page
    {
        protected void sendCSV(MemoryStream CSVData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".csv");

            Response.OutputStream.Write(CSVData.GetBuffer(), 0, (int)CSVData.Length);            
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }        

        protected MemoryStream GenerateCSV(List<Course> courses)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("CourseName, CourseCode, GovernmentCourseCode, GovernmentCourseID, DatabaseID, School, OfferedInSchool, SchoolExam");
            writer.WriteLine(headingLine.ToString());

            /* Data */
            
            foreach (Course course in courses)
            {
                StringBuilder studentLine = new StringBuilder();                
                studentLine.Append(course.name);
                studentLine.Append(",");
                studentLine.Append(course.courseCode);
                studentLine.Append(",");
                studentLine.Append(course.governmentCode);
                studentLine.Append(",");
                studentLine.Append(course.governmentCourseID);
                studentLine.Append(",");
                studentLine.Append(course.id);
                studentLine.Append(",");
                studentLine.Append(course.school);
                studentLine.Append(",");
                studentLine.Append(LSKYCommon.boolToYesOrNo(course.offeredInSchool));
                studentLine.Append(",");
                studentLine.Append(LSKYCommon.boolToYesOrNo(course.schoolExam));
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();            
            return csvFile;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load all courses */
            List<Course> allCourses = new List<Course>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allCourses = Course.loadAllCourses(connection);
            }

            sendCSV(GenerateCSV(allCourses), "LSKY_COURSES_" + LSKYCommon.getCurrentTimeStampForFilename());

        }
    }
}