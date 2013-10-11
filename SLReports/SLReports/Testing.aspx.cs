using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLReports;

namespace SLReports
{
    public partial class Testing : System.Web.UI.Page
    {
        private void displayString(string message)
        {
            Response.Write("<BR>" + message);
        }

        private void displayList<T>(string title, List<T> list)
        {
            displayString("<B>" + title + ": (" + list.Count.ToString() + ")</B>");
            foreach (object obj in list)
            {
                displayString("&nbsp;&nbsp;->&nbsp;" + obj.ToString());
            }
        }
                
                

        private string generateTimeTableTable(List<TimeTableEntry> timetable, List<SchoolDay> schoolDays)
        {
            StringBuilder returnMe = new StringBuilder();

            

            return returnMe.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string selectedStudentID = "12646";
            int selectedReportPeriodID = 479;            

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                Student selectedStudent = Student.loadThisStudent(connection, selectedStudentID);                
                displayString("<B>Selected Student: </B>" + selectedStudent);

                // Derive the term to get classes from
                Term selectedTerm = Term.loadTermFromReportPeriod(connection, selectedReportPeriodID);                
                displayString("<B>Term:</B> " + selectedTerm);

                // Track is already loaded from the student load function
                displayString("<B>Student Track: </B>" + selectedStudent.track);
                
                // Load school days for the track
                List<SchoolDay> schoolDays = SchoolDay.loadDaysFromThisTrack(connection, selectedStudent.track);
                displayList("School Days", schoolDays);

                // Load student timetable entries
                selectedStudent.TimeTable = TimeTableEntry.loadStudentTimeTable(connection, selectedStudent, selectedTerm);

                selectedStudent.TimeTable.getHighestDayNumber();
                
                displayList("Student Timetable", selectedStudent.TimeTable);

                litOutput.Text = generateTimeTableTable(selectedStudent.TimeTable, schoolDays);
            }

            
            

            // Load a student's time table into a multidimensional array

            // Display the time table


            displayString("<BR><BR>");
            stopwatch.Stop();
            displayString("Elapsed time: " + stopwatch.Elapsed);
        }
    }
}