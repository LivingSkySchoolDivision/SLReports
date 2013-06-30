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

namespace SLReports.StudentList
{
    public partial class getCSV : System.Web.UI.Page
    {
        protected void sendCSV(MemoryStream CSVData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".csv");

            Response.OutputStream.Write(CSVData.GetBuffer(), 0, CSVData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        protected MemoryStream GenerateCSV(List<Student> students)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("StudentID, GivenName, MiddleName, Surname, GovernmentID, Grade, Gender, DateOfBirth, HomeRoom, InStatus, InStatusCode, InDate, ResidesInProvince, ResidesInCity, LDAPUserName");
            writer.WriteLine(headingLine.ToString());

            /* Data */
            StringBuilder studentLine = new StringBuilder();
            foreach (Student student in students)
            {
                studentLine.Clear();
                studentLine.Append(student.getStudentID());
                studentLine.Append(",");
                studentLine.Append(student.getGivenName());
                studentLine.Append(",");
                studentLine.Append(student.getMiddleName());
                studentLine.Append(",");
                studentLine.Append(student.getSN());
                studentLine.Append(",");
                studentLine.Append(student.getGovernmentID());
                studentLine.Append(",");
                studentLine.Append(student.getGrade());
                studentLine.Append(",");
                studentLine.Append(student.getGender());
                studentLine.Append(",");
                studentLine.Append(student.getDateOfBirth().ToShortDateString());
                studentLine.Append(",");
                studentLine.Append(student.getHomeRoom());
                studentLine.Append(",");
                studentLine.Append(student.getInStatus());
                studentLine.Append(",");
                studentLine.Append(student.getInStatusCode());               
                studentLine.Append(",");
                studentLine.Append(student.getEnrollDate().ToShortDateString());
                studentLine.Append(",");
                studentLine.Append(student.getRegion());
                studentLine.Append(",");
                studentLine.Append(student.getCity());
                studentLine.Append(",");
                studentLine.Append(student.LDAPUserName);
                writer.WriteLine(studentLine.ToString());
                
            }

            return csvFile;

        }

        private void displayError(string error)
        {
            Response.Write(error);
        }


        private string getDateTimeStamp()
        {
            return DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute; 
        }

        private string removeSpaces(string working)
        {
            try
            {
                return Regex.Replace(working, @"[^\w]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;            

            /* Get school ID from query string */
            if (!String.IsNullOrEmpty(Request.QueryString["schoolid"]))
            {
                int SelectedSchoolID = -1;
                if (int.TryParse(Request.QueryString["schoolid"], out SelectedSchoolID))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        School selectedSchool = School.loadThisSchool(connection, SelectedSchoolID);

                        if (selectedSchool != null)
                        {

                            /* Load students */
                            List<Student> displayedStudents = new List<Student>();

                            displayedStudents = Student.loadStudentsFromThisSchool(connection, int.Parse(selectedSchool.getGovID()));

                            /* Create the CSV */
                            if (displayedStudents.Count > 0)
                            {
                                sendCSV(GenerateCSV(displayedStudents), "STUDENTS_" + removeSpaces(selectedSchool.getName()) + "_" + getDateTimeStamp());
                            }
                            else
                            {
                                displayError("No students were found at this school");
                            }
                        }
                        else
                        {
                            displayError("Invalid school ID");
                        }
                    }
                }
                else
                {
                    displayError("Invalid school ID");
                }

            }
            else
            {
                displayError("SchoolID is required");

            }

        }
    }
}