using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentStatuses
{
    public partial class StatusesOnSameDay : System.Web.UI.Page
    {
        
        String addStudentRow(Student student)
        {
            StringBuilder studentRow = new StringBuilder();

            studentRow.Append("<h3>" + student.getDisplayName() + " <i>(ID #:" + student.getStudentID() + ", School: " + student.getSchoolName() + ",  Active Status ID: " + student.activeStatusID + " )</i></h3>");

            return studentRow.ToString();
        }


        String addStatusRow(Student student, StudentStatus status)
        {

            StringBuilder statusTableContent = new StringBuilder();

            string extraCSS = "";

            if (status.id == student.activeStatusID)
            {
                extraCSS = "border: 2px solid green; background-color: rgba(0,255,0,0.1);";
            }

            statusTableContent.Append("<table style=\"width: 800px; margin-right: 0; margin-left: auto; " + extraCSS + "\">");
            statusTableContent.Append("<tr class=\"datatable_header\">");
            statusTableContent.Append("<td width=50>ID #</td ><td>School</td> <td width=150>InDate</td> <td width=150>InStatus</td> <td width=150>OutDate</td> <td width=150>OutStatus</tD>");
            statusTableContent.Append("</tr>");
            statusTableContent.Append("<tr>");
            statusTableContent.Append("<td>" + status.id + "</td><td>" + status.schoolName + "</td><td>" + status.inDate + "</td><td>" + status.inStatus + "</td><td>" + status.outDate + "</td><td>" + status.outStatus + "</tD>");
            statusTableContent.Append("</tr>");
            statusTableContent.Append("</table>");

            return statusTableContent.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents;
            List<StudentStatus> allStatuses;
            List<Student> displayedStudents = new List<Student>();
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allStudents = Student.loadAllStudents(connection);
                allStatuses = StudentStatus.loadAllStudentStatuses(connection);
            }

            // Map each status to a student
            foreach (StudentStatus status in allStatuses)
            {                
                foreach (Student student in allStudents)
                {
                    if (status.studentNumber == student.getStudentID())
                    {
                        student.statuses.Add(status);
                    }
                }                
            }
            
            // Find problematic statuses
            foreach (Student student in allStudents)
            {
                List<StudentStatus> problematicStatuses = new List<StudentStatus>();
                                
                foreach (StudentStatus status1 in student.statuses)
                {
                    if (status1.outDate != DateTime.MinValue)
                    {
                        foreach (StudentStatus status2 in student.statuses)
                        {
                            if (status1.outDate == status2.inDate)
                            {
                                problematicStatuses.Add(status1);
                                problematicStatuses.Add(status2);
                            }
                        }
                    }
                }

                

                student.statuses = problematicStatuses;
                
                if (student.statuses.Count > 1)
                {
                    displayedStudents.Add(student);
                }
            }

            if (displayedStudents.Count > 0)
            {

                litStudents.Text += "<h3>Found students: " + displayedStudents.Count + "</h3><BR><BR>";

                foreach (Student student in displayedStudents)
                {
                    litStudents.Text += addStudentRow(student);
                    foreach (StudentStatus status in student.statuses)
                    {
                        litStudents.Text += addStatusRow(student, status);
                        litStudents.Text += "<BR>";
                    }
                }
            }
            else
            {
                litStudents.Text += "<i>None</i>";
            }
        }
    }
}