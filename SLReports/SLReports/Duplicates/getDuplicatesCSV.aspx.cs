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

namespace SLReports.Duplicates
{
    public partial class getDuplicatesCSV : System.Web.UI.Page
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

        protected MemoryStream GenerateCSV(Dictionary<Student, String> students)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("StudentNumber, GivenName, MiddleName, Surname, GovernmentID, Gender, DateOfBirth, School, Reason");
            writer.WriteLine(headingLine.ToString());

            /* Data */

            foreach (KeyValuePair<Student, String> student in students)
            {
                StringBuilder studentLine = new StringBuilder();
                studentLine.Append(student.Key.getStudentID());
                studentLine.Append(",");
                studentLine.Append(student.Key.getFirstName());
                studentLine.Append(",");
                studentLine.Append(student.Key.getLegalMiddleName());
                studentLine.Append(",");
                studentLine.Append(student.Key.getLastName());
                studentLine.Append(",");
                studentLine.Append(student.Key.getGovernmentID());
                studentLine.Append(",");
                studentLine.Append(student.Key.getGender());
                studentLine.Append(",");
                studentLine.Append(student.Key.getDateOfBirth().ToShortDateString());
                studentLine.Append(",");
                studentLine.Append(student.Key.getSchoolName());
                studentLine.Append(",");
                studentLine.Append(student.Value);
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            Dictionary<string, Student> workspace_govid = new Dictionary<string, Student>();
            Dictionary<string, Student> workspace_names = new Dictionary<string, Student>();
            Dictionary<Student, string> allDupesWithReason = new Dictionary<Student, string>();

            foreach (Student student in allStudents)
            {
                if (!string.IsNullOrEmpty(student.getGovernmentID()))
                {
                    if (!workspace_govid.ContainsKey(student.getGovernmentID()))
                    {
                        workspace_govid.Add(student.getGovernmentID(), student);
                    }
                    else
                    {
                        allDupesWithReason.Add(student, "Same government ID as " + workspace_govid[student.getGovernmentID()].getStudentID() + " (" + workspace_govid[student.getGovernmentID()].getDisplayName() + ")");
                        allDupesWithReason.Add(workspace_govid[student.getGovernmentID()], "Same government ID as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                    }
                }

                String condensedName = student.getDisplayName().ToLower().Replace(" ", string.Empty).Replace("-", string.Empty);
                if (!workspace_names.ContainsKey(condensedName))
                {
                    workspace_names.Add(condensedName, student);
                }
                else
                {

                    if (student.getDateOfBirth().ToShortDateString() == workspace_names[condensedName].getDateOfBirth().ToShortDateString())
                    {
                        if (!allDupesWithReason.ContainsKey(student))
                        {
                            allDupesWithReason.Add(student, "Same name and date of birth as " + workspace_names[condensedName].getStudentID() + " (" + workspace_names[condensedName].getDisplayName() + ")");
                        }

                        if (!allDupesWithReason.ContainsKey(workspace_names[condensedName]))
                        {
                            allDupesWithReason.Add(workspace_names[condensedName], "Same name and date of birth as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                        }
                    }
                    else
                    {
                        if (!allDupesWithReason.ContainsKey(student))
                        {
                            allDupesWithReason.Add(student, "Same name as " + workspace_names[condensedName].getStudentID() + " (" + workspace_names[condensedName].getDisplayName() + ")");
                        }
                        if (!allDupesWithReason.ContainsKey(workspace_names[condensedName]))
                        {
                            allDupesWithReason.Add(workspace_names[condensedName], "Same name as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                        }

                    }

                }
            }



            sendCSV(GenerateCSV(allDupesWithReason), "LSKY_DUPLICATES_" + LSKYCommon.getCurrentTimeStampForFilename());            

        }
    }
}