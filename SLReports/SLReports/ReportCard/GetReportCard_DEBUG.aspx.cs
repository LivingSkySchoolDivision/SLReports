using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class GetReportCard_DEBUG : System.Web.UI.Page
    {
        // So that the database can be quickly changed
        string sqlConnectionString = PDFReportCardParts.ReportCardDatabase;


        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>();
            List<Student> displayedStudents = new List<Student>();
            List<ReportPeriod> selectedReportPeriods = new List<ReportPeriod>();

            bool anonymize = false;
            if (!string.IsNullOrEmpty(Request.QueryString["anon"]))
            {
                anonymize = true;
            }


            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                // Parse student IDs
                if (!string.IsNullOrEmpty(Request.QueryString["students"]))
                {

                    foreach (string student in Request.QueryString["students"].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(student))
                        {
                            int student_id = -1;
                            if (int.TryParse(student, out student_id))
                            {
                                students.Add(Student.loadThisStudent(connection, student_id.ToString()));
                            }
                        }
                    }

                    foreach (string rp in Request.QueryString["reportperiods"].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(rp))
                        {
                            int rp_id = -1;
                            if (int.TryParse(rp, out rp_id))
                            {
                                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, rp_id));
                            }
                        }
                    }
                }
            }

            selectedReportPeriods.Sort();

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                foreach (Student student in students)
                {
                    if (student != null)
                    {
                        Stopwatch studentStopWatch = new Stopwatch();
                        studentStopWatch.Start();
                        displayedStudents.Add(LSKYCommon.loadStudentMarkData(connection, student, selectedReportPeriods));
                        studentStopWatch.Stop();
                    }
                }
                students.Clear();
            }

            
            // Display data
            Response.Write("<BR><B>DEBUG DATA</b>");
            foreach (Student student in displayedStudents)
            {
                Response.Write("<BR><hr><BR><b>" + student + "</B>");                                
                Response.Write("<BR><b>Absense entries: </b>" + student.absences.Count);
                Response.Write("<BR>&nbsp;<b>Track:</b> " + student.track);
                foreach (Term term in student.track.terms)
                {                    
                    Response.Write("<BR>&nbsp;<b>Term:</b> " + term);
                    foreach (ReportPeriod rp in term.ReportPeriods)
                    {
                        Response.Write("<BR>&nbsp;&nbsp;<b>Report Period:</b> " + rp);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Marks: </b> " + rp.marks.Count);
                        foreach (Mark mark in rp.marks)
                        {
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Mark: </b> " + mark);
                        }
                    }
                    Response.Write("<BR><BR>&nbsp;&nbsp;<b>Classes:</b> " + term.Courses.Count);
                    foreach (SchoolClass c in term.Courses)
                    {
                        Response.Write("<BR><BR>&nbsp;&nbsp;<b>Class:</b> " + c);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Teacher:</b> " + c.teacherName);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Marks:</b> " + c.Marks.Count);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Outcomes:</b> " + c.Outcomes.Count);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Outcome Marks:</b> " + c.OutcomeMarks.Count);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Life Skills:</b> " + c.LifeSkills.Count);
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Life Skills Marks:</b> " + c.LifeSkillMarks.Count);                    
                        foreach (ReportPeriod rp in term.ReportPeriods)
                        {
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Report Period:</b> " + rp);
                            foreach (Mark m in c.Marks)
                            {
                                if (m.reportPeriodID == rp.ID)
                                {
                                    Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Mark</b> " + m);
                                }
                            }
                            foreach (OutcomeMark om in c.OutcomeMarks)
                            {
                                if (om.reportPeriodID == rp.ID)
                                {
                                    Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>OutcomeMark:</b> " + om);
                                }
                            }
                        }
                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Outcomes:</b> " + c.Outcomes.Count);
                        foreach (Outcome o in c.Outcomes)
                        {
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Outcome:</b> " + o);
                            foreach (OutcomeMark om in o.marks)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>OutcomeMark:</b> " + om);
                            }
                        }

                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Life Skills / SLBs:</b> " + c.LifeSkills.Count);
                        foreach (Outcome o in c.LifeSkills)
                        {
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Outcome</b> " + o);
                            foreach (OutcomeMark om in o.marks)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>OutcomeMark:</b> " + om);
                            }
                        }
                      
                    }
                }
          
            }

            



        }
    }
}