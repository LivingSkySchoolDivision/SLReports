using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.FirstClassOfTheDay
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

            Response.OutputStream.Write(CSVData.GetBuffer(), 0, (int)CSVData.Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        protected MemoryStream GenerateCSV(List<Student> students, List<SchoolDay> days)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            // CSV Headings
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("StudentNumber, FirstName, LastName, ");

            for (int x = 0; x < days.Count; x++)
            {
                headingLine.Append(days[x].name);
                if ((x + 1) < days.Count)
                {
                    headingLine.Append(",");
                }
            }
            writer.WriteLine(headingLine.ToString());
            
            // CSV Data
            foreach (Student student in students)
            {                
                StringBuilder studentLine = new StringBuilder();
                studentLine.Append(student.getStudentID());
                studentLine.Append(",");

                studentLine.Append(student.getFirstName());
                studentLine.Append(",");

                studentLine.Append(student.getLastName());
                studentLine.Append(",");

                for (int x = 0; x < days.Count; x++)
                {
                    List<TimeTableEntry> thisDayClasses = new List<TimeTableEntry>();
                                        
                    foreach (TimeTableEntry tte in student.TimeTable)
                    {
                        if (tte.dayNum == days[x].dayNumber)
                        {
                            thisDayClasses.Add(tte);
                        }
                    }

                    if (thisDayClasses.Count > 0)
                    {
                        TimeTableEntry firstClassOfThedDay = TimeTableEntry.getEarliest(thisDayClasses);
                        studentLine.Append("\"" + firstClassOfThedDay.ToStringFormatted() + "\"");
                    }
                    else
                    {
                        studentLine.Append("\"No classes today\"");
                    }

                    
                    if ((x + 1) < days.Count)
                    {
                        studentLine.Append(",");
                    }
                }
                
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }

        private void DisplayError(string error)
        {
            Response.Clear();
            Response.Write("<B>Error: </B>" + error);
            Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["termid"]))
            {
                if (!String.IsNullOrEmpty(Request.QueryString["schoolid"]))
                {

                    if (!String.IsNullOrEmpty(Request.QueryString["trackid"]))
                    {
                        int termID = -1;
                        int trackID = -1;
                        int schoolID = -1;

                        if (int.TryParse(Request.QueryString["schoolid"], out schoolID))
                        {
                            if (int.TryParse(Request.QueryString["termid"], out termID))
                            {
                                if (int.TryParse(Request.QueryString["trackid"], out trackID))
                                {
                                    using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                                    {
                                        // Load school
                                        School selectedSchool = School.loadThisSchool(connection, schoolID);

                                        if (selectedSchool != null)
                                        {
                                            // Load term
                                            Term selectedTerm = Term.loadThisTerm(connection, termID);

                                            // Load track
                                            if (selectedTerm != null)
                                            {
                                                Track selectedTrack = Track.loadThisTrack(connection, trackID);

                                                // Load students
                                                if (selectedTrack != null)
                                                {
                                                    List<Student> selectedStudents = Student.loadStudentsFromThisTrack(connection, selectedTrack.ID);

                                                    // Load timetable data

                                                    List<SchoolDay> schoolDays = SchoolDay.loadDaysFromThisTrack(connection, selectedTrack);

                                                    foreach (Student student in selectedStudents)
                                                    {
                                                        student.TimeTable = TimeTableEntry.loadStudentTimeTable(connection, student, selectedTerm);
                                                    }

                                                    string filename = "FirstClassOfDay_" + LSKYCommon.removeSpaces(selectedSchool.getName()) + "_" + LSKYCommon.removeSpaces(selectedTerm.name);
                                                    sendCSV(GenerateCSV(selectedStudents, schoolDays), filename);
                                                    
                                                }
                                                else
                                                {
                                                    DisplayError("Track not found");
                                                }
                                            }
                                            else
                                            {
                                                DisplayError("Term not found");
                                            }
                                        }
                                        else
                                        {
                                            DisplayError("School not found");
                                        }

                                    }


                                }
                                else
                                {
                                    DisplayError("Invalid track");
                                }
                            }
                            else
                            {
                                DisplayError("Invalid term");
                            }   
                        }
                        else
                        {
                            DisplayError("Invalid school");
                        }                    

                    }
                    else
                    {
                        DisplayError("Track must be specified");
                    }
                }
                else
                {
                    DisplayError("School must be specified");
                }
            }
            else
            {
                DisplayError("Term must be specified");
            }
        }



    }
}