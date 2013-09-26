using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentList
{
    public partial class allStudentsCSV : System.Web.UI.Page
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

        protected MemoryStream GenerateCSV(List<Student> students)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("School, StudentID, GivenName, MiddleName, Surname, GovernmentID, Grade, Gender, DateOfBirth, HomeRoom, InStatus, InStatusCode, InDate, Telephone, Apartment, House, Street, City, Province, PostalCode, LDAPUserName");
            writer.WriteLine(headingLine.ToString());

            /* Data */
            StringBuilder studentLine = new StringBuilder();
            foreach (Student student in students)
            {
                studentLine.Clear();
                studentLine.Append(student.getSchoolName());
                studentLine.Append(",");
                studentLine.Append(student.getStudentID());
                studentLine.Append(",");
                studentLine.Append(student.getFirstName());
                studentLine.Append(",");
                studentLine.Append(student.getLegalMiddleName());
                studentLine.Append(",");
                studentLine.Append(student.getLastName());
                studentLine.Append(",");
                studentLine.Append(student.getGovernmentID());
                studentLine.Append(",");
                studentLine.Append(student.getGrade());
                studentLine.Append(",");
                studentLine.Append(student.getGender());
                studentLine.Append(",");
                studentLine.Append("\"" + student.getDateOfBirth().ToShortDateString() + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + student.getHomeRoom() + "\"");
                studentLine.Append(",");
                studentLine.Append(student.getInStatus());
                studentLine.Append(",");
                studentLine.Append(student.getInStatusCode());
                studentLine.Append(",");
                studentLine.Append("\"" + student.getEnrollDate().ToShortDateString() + "\"");
                studentLine.Append(",");
                studentLine.Append(student.getTelephone());
                studentLine.Append(",");
                studentLine.Append("\"" + student.getApartmentNo() + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + student.getHouseNo() + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + student.getStreet() + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + student.getCity() + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + student.getRegion() + "\"");
                studentLine.Append(",");
                studentLine.Append(student.getPostalCode());
                studentLine.Append(",");
                studentLine.Append(student.LDAPUserName);

                // Temp
                //studentLine.Append(",");
                //studentLine.Append(student.aborigStatus);


                writer.WriteLine(studentLine.ToString());
            }

            writer.Flush();
            csvFile.Flush();
            return csvFile;

        }

        private void displayError(string error)
        {
            Response.Write(error);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {              

                /* Load students */
                List<Student> displayedStudents = new List<Student>();

                displayedStudents = Student.loadAllStudents(connection);

                /* Create the CSV */
                if (displayedStudents.Count > 0)
                {
                    sendCSV(GenerateCSV(displayedStudents), "STUDENTS_ALLSCHOOLS_" + LSKYCommon.removeSpaces(LSKYCommon.getCurrentTimeStampForFilename()));
                }
                else
                {
                    displayError("No students were found at this school");
                }
            }
                

        }
    }
}