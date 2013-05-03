using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Attendance
{
    public partial class index : System.Web.UI.Page
    {
        public static string selectedStudentID;
        public static Student selectedStudent;
        public static List<Absence> selectedStudentAbsences;

        public static DateTime selectedStartDate;
        public static DateTime selectedEndDate;

        protected void Page_Load(object sender, EventArgs e)
        {

            /* Initialize some variables */
            NameValueCollection post = Request.Form;

            /* Set up an SQL connection */
            String dbUser = @"sql_readonly";
            String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
            String dbHost = "dcsql.lskysd.ca";
            String dbDatabase = "SchoolLogicDB";
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            //NameValueCollection get = Request.QueryString;
            if (!string.IsNullOrEmpty(Request["studentid"]))
            {
                selectedStudentID = Request["studentid"];
            }
            else
            {
                selectedStudentID = null;
            }

            selectedStartDate = new DateTime(1900, 1, 1);
            if (!string.IsNullOrEmpty(Request["from_year"]))
            {
                string year = Request["from_year"];
                if (!string.IsNullOrEmpty(Request["from_month"]))
                {
                    string month = Request["from_month"];
                    if (!string.IsNullOrEmpty(Request["from_day"]))
                    {
                        string day = Request["from_day"];
                        selectedStartDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    }
                }
            }


            selectedEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Request["to_year"]))
            {
                string year = Request["to_year"];
                if (!string.IsNullOrEmpty(Request["to_month"]))
                {
                    string month = Request["to_month"];
                    if (!string.IsNullOrEmpty(Request["to_day"]))
                    {
                        string day = Request["to_day"];
                        selectedEndDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    }
                }
            }

            /* Load configuration information from a config file */
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                if (selectedStudentID != null)
                {
                    selectedStudent = Student.loadThisStudent(connection, selectedStudentID);
                }

                if (selectedStudent != null)
                {
                    selectedStudentAbsences = Student.loadAbsencesFromStudent(connection, selectedStudent.getStudentID(), selectedStartDate, selectedEndDate);
                }
            }

        }

        public void displayStudentNameplate(Student student)
        {
            Response.Write("<table border=0 cellpadding=0 cellspacing=0 style=\"font-family: Arial;\">");
            Response.Write("<tr>");
            Response.Write("<td width=200><b>Student:</b></td><td width=300 style=\"font-size: 10pt;\">" + student.getDisplayName() + "</td>");
            Response.Write("<td width=200><b>Student #:</b></td><td width=200 style=\"font-size: 10pt;\">" + student.getStudentID() + "</td>");
            Response.Write("</tr>");
            Response.Write("<tr>");
            Response.Write("<td><b>School:</b></td><td width=300 style=\"font-size: 10pt;\">" + student.getSchoolName() + "</td>");
            Response.Write("<td ><b>Grade:</b></td><td width=200 style=\"font-size: 10pt;\">" + student.getGrade() + "</td>");
            Response.Write("</tr>");
            Response.Write("<tr>");
            Response.Write("<td><b>Enroll Date:</b></td><td width=300 style=\"font-size: 10pt;\">" + student.getEnrollDate().ToShortDateString() + "</td>");
            Response.Write("<td><b>InStatus:</b></td><td width=200 style=\"font-size: 10pt;\">" + student.getInStatus() + "</td>");
            Response.Write("</tr>");
            Response.Write("</table>");
            Response.Write("<BR>");
        }

        /// <summary>
        /// Sorts a list of absences into a collection by date for easier consumption
        /// </summary>
        /// <param name="absences">A list of absences</param>
        /// <returns>A Dictionary collection of dates (key), with a collection of absences on that date (value)</returns>
        public SortedDictionary<DateTime, List<Absence>> getAbsencesByDate(List<Absence> absences)
        {
            SortedDictionary<DateTime, List<Absence>> absencesByDate = new SortedDictionary<DateTime, List<Absence>>();
            foreach (Absence thisAbs in absences)
            {
                if (!absencesByDate.ContainsKey(thisAbs.getDate()))
                {
                    absencesByDate[thisAbs.getDate()] = new List<Absence>();
                }
                absencesByDate[thisAbs.getDate()].Add(thisAbs);
            }
            return absencesByDate;
        }

        public SortedDictionary<string, List<Absence>> getAbsencesByStatus(List<Absence> absences)
        {
            SortedDictionary<string, List<Absence>> absencesByDate = new SortedDictionary<string, List<Absence>>();
            foreach (Absence thisAbs in absences)
            {
                if (!absencesByDate.ContainsKey(thisAbs.getStatus()))
                {
                    absencesByDate[thisAbs.getStatus()] = new List<Absence>();
                }
                absencesByDate[thisAbs.getStatus()].Add(thisAbs);
            }
            return absencesByDate;
        }

        public SortedDictionary<string, List<Absence>> getAbsencesByReason(List<Absence> absences)
        {
            SortedDictionary<string, List<Absence>> absencesByDate = new SortedDictionary<string, List<Absence>>();
            foreach (Absence thisAbs in absences)
            {
                if (!absencesByDate.ContainsKey(thisAbs.getReason()))
                {
                    absencesByDate[thisAbs.getReason()] = new List<Absence>();
                }
                absencesByDate[thisAbs.getReason()].Add(thisAbs);
            }
            return absencesByDate;
        }



        public void displayAbsenceStatistics(List<Absence> absences)
        {
            Response.Write("<br><br><h3>Absence Statistics: " + selectedStartDate.ToShortDateString() + " to " + selectedEndDate.ToShortDateString() + "</h3>");

            Response.Write("<table style=\"font-size: 10pt;\" border=0 cellpadding=0 cellspacing=0 width=\"500\">");
            Response.Write("<tr><td><b>Status</b></td><td><b>Reason</b></td><td><b>Count</b></td></tr>");
            foreach (KeyValuePair<string, List<Absence>> obj in getAbsencesByStatus(absences))
            {
                string status = obj.Key;
                if (string.IsNullOrEmpty(status))
                    status = "No status specified";
                List<Absence> thisStatusAbsences = obj.Value;
                Response.Write("<tr><td>" + status + "</td><td></td><td><b>" + thisStatusAbsences.Count() + "</b></td></tr>");
                foreach (KeyValuePair<string, List<Absence>> obj2 in getAbsencesByReason(thisStatusAbsences))
                {
                    string reason = obj2.Key;
                    if (string.IsNullOrEmpty(reason))
                        reason = "No reason specified";
                    List<Absence> thisReasonAbsences = obj2.Value;
                    Response.Write("<tr><td></td><td>" + reason + " <i>(" + thisReasonAbsences.Count() + ")</i></td><td></td></tr>");
                }


            }

            /*
            
            Response.Write("<tr><td width=150></td><td></td></tr>");
             */
            Response.Write("</table>");
        }

        public void displayAbsenceListText(List<Absence> absences)
        {
            Response.Write("<br><br><h3>List of Absences: " + selectedStartDate.ToShortDateString() + " to " + selectedEndDate.ToShortDateString() + "</h3>");

            /* Figure out the name of the blocks */
            foreach (KeyValuePair<DateTime, List<Absence>> obj in getAbsencesByDate(absences))
            {
                DateTime thisDate = obj.Key;
                List<Absence> thisDatesAbsences = obj.Value;
                Response.Write("<div style=\"margin-bottom: 10px;\">");
                Response.Write("<div><b>" + thisDate.ToLongDateString() + "</b></div>");
                Response.Write("<div style=\"margin-left: 10px;font-size: 10pt;\">");
                foreach (Absence abs in thisDatesAbsences)
                {
                    Response.Write("<B>" + abs.getStartTime().ToShortTimeString() + "-" + abs.getEndTime().ToShortTimeString() + "</b> ");
                    Response.Write(" - " + abs.getCourseName());
                    Response.Write(" - " + abs.getStatus());
                    if (!string.IsNullOrEmpty(abs.getReason()))
                    {
                        Response.Write("(" + abs.getReason() + ")");
                    }
                    else
                    {
                        Response.Write(" (No reason specified)");
                    }

                    if (!string.IsNullOrEmpty(abs.getComment()))
                    {
                        Response.Write(" (<b>Comment:</b> " + abs.getComment() + ")");
                    }

                    Response.Write(" <br />");
                }
                Response.Write("</div>");
                Response.Write("</div>");

            }

        }

        public void displayAbsenceListTable(List<Absence> absences)
        {
            SortedDictionary<int, string> blocks = new SortedDictionary<int, string>();

            /* Figure out how many blocks there are in a day */
            foreach (Absence abs in absences)
            {
                if (!blocks.ContainsKey(abs.getBlock()))
                {
                    if (string.IsNullOrEmpty(abs.getCourseID()))
                    {
                        blocks.Add(abs.getBlock(), "Block " + abs.getBlock() /*+ "<br>" + abs.getStartTime().ToShortTimeString() + "-" + abs.getEndTime().ToShortTimeString()*/);
                    }
                    else
                    {
                        blocks.Add(abs.getBlock(), "Block " + abs.getBlock() + "<br>" + abs.getCourseName() /*+ "<br>" + abs.getStartTime().ToShortTimeString() + "-" + abs.getEndTime().ToShortTimeString()*/);
                    }
                }
                else
                {
                    blocks[abs.getBlock()] += ", " + abs.getCourseName();
                }
            }


            Response.Write("<H3>Absences</H3>");

            Response.Write("<table border=0 cellpadding=5 cell spacing=0>");
            Response.Write("<tr><td style=\"font-size: 8pt;\"><b>Date</b></td>");
            foreach (KeyValuePair<int, string> block in blocks)
            {
                Response.Write("<td style=\"font-size: 8pt;\"><b>" + block.Value + "</b></td>");
            }
            Response.Write("</tr>");

            /* Figure out the name of the blocks */
            foreach (KeyValuePair<DateTime, List<Absence>> obj in getAbsencesByDate(absences))
            {
                DateTime thisDate = obj.Key;
                List<Absence> thisDatesAbsences = obj.Value;
                Response.Write("<tr>");
                Response.Write("<td width=200 style=\"font-size: 8pt;\">" + thisDate.ToLongDateString() + "</td>");
                foreach (KeyValuePair<int, string> block in blocks)
                {
                    Response.Write("<td style=\"font-size: 8pt;\">");
                    foreach (Absence thisAbsence in thisDatesAbsences)
                    {
                        if (thisAbsence.getBlock().Equals(block.Key))
                        {
                            Response.Write(thisAbsence.getStatus().Trim());
                            if (!string.IsNullOrEmpty(thisAbsence.getReason()))
                            {
                                Response.Write("-" + thisAbsence.getReason().Trim());
                            }
                            //break;
                        }
                    }
                    Response.Write("</td>");
                }
            }
            Response.Write("</table>");
        }


    }
}