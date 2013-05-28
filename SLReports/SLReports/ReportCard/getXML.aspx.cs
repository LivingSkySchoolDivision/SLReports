using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class getXML : System.Web.UI.Page
    {
        Student selectedStudent;        
        School selectedSchool;

        Stopwatch stopWatch;
        

        private string XMLNameSection(Student student)
        {
            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("<Student>");
            returnMe.Append("<GivenName>"+student.getGivenName()+"</GivenName>");
            returnMe.Append("<Surname>" + student.getSN() + "</Surname>");
            returnMe.Append("<MiddleName>" + student.getMiddleName() + "</MiddleName>");
            returnMe.Append("<StudentNumber>" + student.getStudentID() + "</StudentNumber>");
            returnMe.Append("<GovernmentIDNumber>" + student.getGovernmentID() + "</GovernmentIDNumber>");
            returnMe.Append("<Grade>" + student.getGrade() + "</Grade>");
            returnMe.Append("<Gender>" + student.getGender() + "</Gender>");
            returnMe.Append("<HomeRoom>" + student.getHomeRoom() + "</HomeRoom>");
            returnMe.Append("<HasPhoto>" + student.hasPhoto().ToString() + "</HasPhoto>");
            returnMe.Append("<Telephone>" + student.getTelephone() + "</Telephone>");
            returnMe.Append("<Address>");
            returnMe.Append("<HouseNumber>" + student.getHouseNo() + "</HouseNumber>");
            returnMe.Append("<ApartmentNumber>" + student.getApartmentNo() + "</ApartmentNumber>");
            returnMe.Append("<Street>" + student.getStreet() + "</Street>");
            returnMe.Append("<City>" + student.getCity() + "</City>");
            returnMe.Append("<Province>" + student.getRegion() + "</Province>");
            returnMe.Append("<PostalCode>" + student.getPostalCode() + "</PostalCode>");
            returnMe.Append("</Address>");
            returnMe.Append("</Student>");
            return returnMe.ToString();
        }

        private string XMLSchoolSection(School school)
        {
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append("<School>");
            returnMe.Append("<Name>"+school.getName()+"</Name>");
            returnMe.Append("<SchoolID>" + school.getGovID() + "</SchoolID>");
            returnMe.Append("</School>");

            return returnMe.ToString();
        }

        private string escapeXMLSpecialChars(string haystack)
        {            
            return Regex.Replace(haystack, "&", "&amp;");
        }

        private string XMLCourseSection(Student student)
        {
            StringBuilder returnMe = new StringBuilder();


            returnMe.Append("<Track ID=\"" + student.track.ID + "\" Name=\"" + student.track.name + "\">");
            foreach (Term term in student.track.terms)
            {
                returnMe.Append("<Term ID=\"" + term.ID + "\" Name=\"" + term.name + "\">");

                foreach (Course course in term.Courses)
                {
                    returnMe.Append("<Course Name=\"" + course.name + "\">");
                    returnMe.Append("<Name>" + course.name + "</Name>");
                    returnMe.Append("<CourseID>" + course.courseid + "</CourseID>");
                    returnMe.Append("<ClassID>" + course.classid + "</ClassID>");
                    returnMe.Append("<HasObjectives>" + course.hasObjectives() + "</HasObjectives>");
                    returnMe.Append("<Teacher>");
                    returnMe.Append("<GivenName>" + course.teacherFirstName + "</GivenName>");
                    returnMe.Append("<Surname>" + course.teacherLastName + "</Surname>");
                    returnMe.Append("<Title>" + course.teacherTitle + "</Title>");
                    returnMe.Append("</Teacher>");
                    
                    returnMe.Append("<Marks>");
                    foreach (ReportPeriod reportPeriod in course.ReportPeriods)
                    {                        
                        returnMe.Append("<ReportPeriod ID=\"" + reportPeriod.ID + "\" Name=\"" + reportPeriod.name + "\">");                        
                        foreach (Mark mark in course.Marks)
                        {
                            
                            if (mark.reportPeriod.ID == reportPeriod.ID)
                            {
                                returnMe.Append("<Mark>");
                                returnMe.Append("<AlphaMark>" + mark.outcomeMark + "</AlphaMark>");
                                returnMe.Append("<PercentMark>" + mark.numberMark + "</PercentMark>");
                                returnMe.Append("<Comment>" + mark.comment + "</Comment>");
                                returnMe.Append("</Mark>");
                            }
                        }
                        returnMe.Append("</ReportPeriod>");
                    }
                    returnMe.Append("</Marks>");
                    returnMe.Append("<Objectives>");
                    foreach (ReportPeriod reportPeriod in course.ReportPeriods)
                    {
                        returnMe.Append("<ReportPeriod ID=\"" + reportPeriod.ID + "\" Name=\"" + reportPeriod.name + "\">");
                        foreach (ObjectiveMark objectivem in course.ObjectiveMarks)
                        {
                            if (objectivem.reportPeriodID == reportPeriod.ID)
                            {

                                returnMe.Append("<Objective ID=\"" + objectivem.objective.id + "\">");
                                returnMe.Append("<Subject>" + escapeXMLSpecialChars(objectivem.objective.subject) + "</Subject>");
                                returnMe.Append("<Description>" + escapeXMLSpecialChars(objectivem.objective.description) + "</Description>");
                                returnMe.Append("<Mark>" + objectivem.mark + "</Mark>");
                                returnMe.Append("</Objective>");
                            }
                        }
                        returnMe.Append("</ReportPeriod>");
                    }
                    returnMe.Append("</Objectives>");

                    returnMe.Append("<LifeSkills>");
                    returnMe.Append("</LifeSkills>");

                    returnMe.Append("</Course>");
                }
                
                returnMe.Append("</Term>"); 
                
            }
            returnMe.Append("</Track>");
            return returnMe.ToString();
        }

        private string XMLAttendanceSection()
        {
            StringBuilder returnMe = new StringBuilder();
            return returnMe.ToString();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            selectedStudent = null;
            selectedSchool = null;

            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
                {
                    int studentID = -1;
                    if (int.TryParse(Request.QueryString["studentid"], out studentID))
                    {
                        using (SqlConnection connection = new SqlConnection(dbConnectionString))
                        {
                            selectedStudent = Student.loadThisStudent(connection, studentID.ToString());

                            if (selectedStudent != null)
                            {
                                selectedSchool = School.loadThisSchool(connection, int.Parse(selectedStudent.getSchoolID()));


                                /* Get student track, and determine the terms and report periods */
                                selectedStudent.track = Track.loadThisTrack(connection, int.Parse(selectedStudent.getTrackID()));

                                /* Populate the track with terms */
                                selectedStudent.track.terms = Term.loadTermsFromThisTrack(connection, selectedStudent.track);

                                /* Populate the terms with report periods */
                                foreach (Term t in selectedStudent.track.terms)
                                {
                                    List<ObjectiveMark> TermObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisStudent(connection, t, selectedStudent);

                                    t.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, t);

                                    Dictionary<int, Course> termCourses = new Dictionary<int, Course>();
                                    termCourses.Clear();

                                    /* Load marks into the report period */
                                    foreach (ReportPeriod r in t.ReportPeriods)
                                    {
                                        r.marks = Mark.loadMarksFromThisReportPeriod(connection, r, selectedStudent);

                                        Dictionary<int, Course> allcourses = new Dictionary<int, Course>();
                                        foreach (Mark m in r.marks)
                                        {
                                            if (!allcourses.ContainsKey(m.courseID))
                                            {
                                                allcourses.Add(m.courseID, new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                            }

                                            if (!termCourses.ContainsKey(m.courseID))
                                            {
                                                termCourses.Add(m.courseID, new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                            }
                                        }


                                        foreach (KeyValuePair<int, Course> kvp in termCourses)
                                        {
                                            Course c = kvp.Value;
                                            Dictionary<int, ReportPeriod> detectedReportPeriods = new Dictionary<int, ReportPeriod>();

                                            foreach (Mark m in r.marks)
                                            {
                                                if (!detectedReportPeriods.ContainsKey(m.reportPeriodID))
                                                {
                                                    detectedReportPeriods.Add(m.reportPeriodID, m.reportPeriod);
                                                }
                                            }

                                            foreach (KeyValuePair<int, ReportPeriod> drp in detectedReportPeriods)
                                            {
                                                c.ReportPeriods.Add(drp.Value);


                                            }

                                            foreach (Mark m in r.marks)
                                            {
                                                if (m.courseID == c.courseid)
                                                {
                                                    c.Marks.Add(m);
                                                }
                                            }


                                            c.ObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisCourse(connection, t, selectedStudent, c);
                                            foreach (ObjectiveMark om in c.ObjectiveMarks)
                                            {
                                                foreach (Objective o in c.Objectives)
                                                {
                                                    if (om.objectiveID == o.id)
                                                    {
                                                        om.objective = o;
                                                    }
                                                }
                                            }

                                            c.Objectives = Objective.loadObjectivesForThisCourse(connection, c);
                                            foreach (Objective o in c.Objectives)
                                            {
                                                foreach (ObjectiveMark om in TermObjectiveMarks)
                                                {
                                                    if (om.objectiveID == o.id)
                                                    {
                                                        o.mark = om;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    foreach (KeyValuePair<int, Course> kvp in termCourses)
                                    {
                                        t.Courses.Add(kvp.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

            if (selectedStudent != null)
            {
                Response.Clear();                
                Response.ContentEncoding = Encoding.UTF8;
                Response.ContentType = "text/xml; charset=utf-8";
                Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                Response.Write("<ReportCard>");
                Response.Write(XMLSchoolSection(selectedSchool));
                Response.Write(XMLNameSection(selectedStudent));
                Response.Write(XMLCourseSection(selectedStudent));
                Response.Write("</ReportCard>");
                Response.End();
            }
            else
            {
                Response.Write("Invalid student specified");
            }
        }
    }
}