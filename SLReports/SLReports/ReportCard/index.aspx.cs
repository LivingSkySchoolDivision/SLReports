using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class index : System.Web.UI.Page
    {
        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";
        
        List<School> AllSchools;
        List<Student> DisplayedStudents;
        List<Term> DisplayedTerms;
        List<ReportPeriod> DisplayedReportPeriods;
        List<Mark> DisplayedMarks;
        List<ObjectiveMark> StudentObjectiveMarks;

        Student SelectedStudent;
        int SelectedSchoolID;
        Term SelectedTerm;

        protected void Page_Init(object sender, EventArgs e)
        {
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Load all schools */
                AllSchools = School.loadAllSchools(connection);
            }

            foreach (School school in AllSchools)
            {
                ListItem newItem = new ListItem();
                newItem.Text = school.getName();
                newItem.Value = school.getGovID();
                drpSchoolList.Items.Add(newItem);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load a specific student, for testing */
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {

                SelectedStudent = Student.loadThisStudent(connection, "12511");

                if (!String.IsNullOrEmpty(SelectedStudent.getTrackID()))
                {
                    Track selectedTrack = Track.loadThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                    List<Term> validTerms = Term.loadTermsFromThisTrack(connection, selectedTrack.getID());

                    if (selectedTrack != null)
                    {                        
                        foreach (Term term in validTerms)
                        {                            
                            List<ReportPeriod> reportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection,term.ID);
                            foreach (ReportPeriod period in reportPeriods)
                            {                                
                                List<Mark> marks = Mark.loadMarksFromThisReportPeriod(connection, period, SelectedStudent);                                
                                foreach (Mark mark in marks)
                                {                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        Response.Write("No Track");
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                drpStudentList.Items.Clear();

                foreach (Student student in DisplayedStudents)
                {
                    ListItem newItem = new ListItem();
                    newItem.Value = student.getStudentID();
                    newItem.Text = student.getStudentID() + " " + student.getDisplayName();
                    drpStudentList.Items.Add(newItem);
                }
                TableRow_Student.Visible = true;
                TableRow_Term.Visible = false;
                litAttendance.Visible = false;
                litMarks.Visible = false;
                litNamePlate.Visible = false;
                
            }            
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);

            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                DisplayedTerms = Term.loadTermsFromThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                drpTermList.Items.Clear();
                foreach (Term term in DisplayedTerms)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = term.name;
                    newItem.Value = term.ID.ToString();
                    drpTermList.Items.Add(newItem);
                }
                TableRow_Term.Visible = true;
                litAttendance.Visible = false;
                litMarks.Visible = false;
                litNamePlate.Visible = false;

            }            
            
        }
                
        protected string generateMarkTable(List<Course> courses)
        {
            List<ReportPeriod> validReportPeriods = new List<ReportPeriod>();
            foreach (Course c in courses)
            {
                foreach (Mark m in c.Marks)
                {
                    if (!validReportPeriods.Contains(m.reportPeriod))
                    {
                        validReportPeriods.Add(m.reportPeriod);
                    }
                }
            }

            StringBuilder returnMe = new StringBuilder();                                   

            returnMe.Append("<table width=\"100%\">");
            

            
            /* Content */
            foreach (Course course in courses)
            {
                bool courseHasComments = false;

                returnMe.Append("<tr class=\"datatable_header\">");
                returnMe.Append("<th style=\"text-align: left;\"></th>");
                foreach (ReportPeriod rp in validReportPeriods)
                {
                    returnMe.Append("<th>" + rp.name + "</th>");
                }
                returnMe.Append("</tr>");
                returnMe.Append("<tr class=\"datatable_sub_header\">");
                returnMe.Append("<td><b style=\"font-size: 125%\">" + course.name + "</b> - " + course.teacherName + "</td>");
                foreach (ReportPeriod rp in validReportPeriods)
                {
                    returnMe.Append("<td align=\"center\" width=\"150\">");
                    foreach (Mark mark in course.Marks)
                    {   
                        if (mark.reportPeriod.ID == rp.ID)
                        {
                            returnMe.Append(mark.getMark());
                        }

                        /* While we are iterating marks, check to see if any have comments */
                        if (!string.IsNullOrEmpty(mark.comment))
                        {
                            courseHasComments = true;
                        }

                    } 
                    returnMe.Append("</td>");
                }
                returnMe.Append("</tr>");

                if (courseHasComments)
                {
                    returnMe.Append("<tr>");
                    returnMe.Append("<td colspan=\"" + (validReportPeriods.Count + 1) + "\">");

                    foreach (Mark mark in course.Marks)
                    {
                        if (mark.classID == course.classid)
                        {
                            if (!string.IsNullOrEmpty(mark.comment))
                            {
                                returnMe.Append("<div style=\"padding-left: 15px;margin-bottom: 5px;\"><b>" + mark.reportPeriod.name + "</b>: " + mark.comment + "</div>");
                            }
                        }
                    }
                    returnMe.Append("</td>");
                    returnMe.Append("</tr>");
                }

                if (course.Objectives.Count > 0)
                {
                    returnMe.Append("<tr>");
                    returnMe.Append("<td colspan=\"" + (validReportPeriods.Count + 1) + "\">");
                    returnMe.Append("<table class=\"datatable\" width=\"100%\" style=\"padding-left: 15px;\" cellpadding=3>");
                    foreach (Objective objective in course.Objectives)
                    {
                        if (objective.mark != null)
                        {
                            returnMe.Append("<tr class=\"row\"><td>" + objective.description + "</td><td width=\"75\" align=\"center\">");
                            returnMe.Append(objective.mark.mark);
                            returnMe.Append("</td></tr>");
                        }
                    }
                    returnMe.Append("</table>");
                    returnMe.Append("</td>");
                    returnMe.Append("</tr>");
                }
                returnMe.Append("<tr><td colspan=\"" + (validReportPeriods.Count + 1) + "\">&nbsp;</td></tr>");
            }
            returnMe.Append("</table>");
            return returnMe.ToString();
        }

        protected string generateAttendanceTable(Student student)
        {
            StringBuilder returnMe = new StringBuilder();




            



            return returnMe.ToString();
        }

        protected string generateStudentNameplate(Student student)
        {
            StringBuilder returnMe = new StringBuilder();

            

            returnMe.Append("<div style=\"width: 500px;margin-left: auto; margin-right: auto;\">");if (student.hasPhoto())
            {
                returnMe.Append("<div style=\"float: left;\"><img width=\"156\" height=\"200\" src=\"/SLReports/Photos/GetPhoto.aspx?studentnumber="+student.getStudentID()+"\"></div>");
            }
            returnMe.Append("<div style=\"text-align: center; border: 0; border-bottom: 1px solid black; font-size: 150%;font-weight: bold;\">" + student.getDisplayName() + "</div>");
            returnMe.Append("<table width=\"300\">");


            returnMe.Append("<tr><td valign=\"top\" width=\"100\"><b>School</b></td><td valign=\"top\">" + student.getSchoolName() + "</td></tr>");

            if (!string.IsNullOrEmpty(student.getHomeRoom()))
            {
                returnMe.Append("<tr><td><b>Homeroom</b></td><td valign=\"top\">" + student.getHomeRoom() + "</tr></td>");
            }
            returnMe.Append("<tr><td valign=\"top\"><b>Grade</b></td><td valign=\"top\">" + student.getGrade() + "</tr></td>");
            returnMe.Append("<tr><td valign=\"top\"><b>Date of Birth</b></td><td valign=\"top\">" + student.getDateOfBirth().ToString("MMMM dd,yyyy") + "</tr></td>");
            returnMe.Append("<tr><td valign=\"top\"><b>Home Phone</b></td><td valign=\"top\">" + student.getTelephoneFormatted() + "</tr></td>");

            returnMe.Append("</table>");
            returnMe.Append("</div><br/>");


            return returnMe.ToString();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);

                String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
                    DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                    DisplayedTerms = Term.loadTermsFromThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                    SelectedTerm = Term.loadThisTerm(connection, int.Parse(drpTermList.SelectedValue));

                    DisplayedReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, SelectedTerm.ID);
                    DisplayedMarks = Mark.loadMarksFromTheseReportPeriods(connection, DisplayedReportPeriods, SelectedStudent);

                    StudentObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisStudent(connection, SelectedTerm, SelectedStudent);

                    List<Course> DisplayedMarksInCourses = new List<Course>();
                    foreach (Mark m in DisplayedMarks)
                    {
                        bool courseExists = false;
                        foreach (Course c in DisplayedMarksInCourses)
                        {
                            if (c.classid == m.classID)
                            {
                                courseExists = true;
                            }
                        }
                        if (!courseExists)
                        {
                            DisplayedMarksInCourses.Add(new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                        }
                    }

                    foreach (Course c in DisplayedMarksInCourses)
                    {
                        c.Objectives = Objective.loadObjectivesForThisCourse(connection, c);
                        foreach (Objective o in c.Objectives)
                        {
                            foreach (ObjectiveMark om in StudentObjectiveMarks)
                            {
                                if (o.id == om.objectiveID)
                                {
                                    o.mark = om;
                                }
                            }
                        }


                        foreach (Mark m in DisplayedMarks)
                        {
                            if (m.classID == c.classid)
                            {
                                c.Marks.Add(m);
                            }
                        }
                    }

                    litNamePlate.Text = generateStudentNameplate(SelectedStudent);
                    litMarks.Text = generateMarkTable(DisplayedMarksInCourses);
                    litAttendance.Text = generateAttendanceTable(SelectedStudent);
                    
                    litAttendance.Visible = true;
                    litMarks.Visible = true;
                    litNamePlate.Visible = true;

                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<BR>");
            }

        }
    }
}