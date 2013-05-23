using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class getXML : System.Web.UI.Page
    {
        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";

        Student selectedStudent;
        ReportPeriod selectedReportPeriod;
        School selectedSchool;
        List<Term> studentMarks;

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

        private string XMLCourseSection(Student student)
        {
            StringBuilder returnMe = new StringBuilder();


            returnMe.Append("<Track ID=\"" + student.track.ID + "\" Name=\"" + student.track.name + "\">");
            foreach (Term term in student.track.terms)
            {
                returnMe.Append("<Term ID=\"" + term.ID + "\" Name=\"" + term.name + "\">");

                foreach (ReportPeriod reportPeriod in term.ReportPeriods)
                {
                    returnMe.Append("<ReportingPeriod ID=\"" + reportPeriod.ID + "\" Name=\"" + reportPeriod.name + "\">");

                    foreach (Course course in reportPeriod.courses)
                    {
                        returnMe.Append("<Course>");
                        returnMe.Append("<Name>" + course.name + "</Name>");
                        returnMe.Append("<CourseID>" + course.courseid + "</CourseID>");
                        returnMe.Append("<ClassID>" + course.classid + "</ClassID>");
                        returnMe.Append("<HasOutcomes>" + course.hasObjectives() + "</HasOutcomes>");
                        returnMe.Append("<Teacher>");
                        returnMe.Append("<GivenName>" + course.teacherFirstName + "</GivenName>");
                        returnMe.Append("<Surname>" + course.teacherLastName + "</Surname>");
                        returnMe.Append("<Title>" + course.teacherTitle + "</Title>");
                        returnMe.Append("</Teacher>");

                        returnMe.Append("<Marks>");
                        foreach (Mark mark in course.Marks)
                        {
                            returnMe.Append("<Mark>");
                            returnMe.Append("<AlphaMark>" + mark.outcomeMark + "</AlphaMark>");
                            returnMe.Append("<PercentMark>" + mark.numberMark + "</PercentMark>");
                            returnMe.Append("<Comment>" + mark.comment + "</Comment>");
                            returnMe.Append("</Mark>");                            
                        }

                        returnMe.Append("</Marks>");
                        returnMe.Append("<Objectives>");
                        foreach (Objective objective in course.Objectives)
                        {
                            returnMe.Append("<Objective ID=\"" + objective.id + "\">");
                            returnMe.Append("<Subject>" + @objective.subject + "</Subject>");
                            returnMe.Append("<Description>" + @objective.description + "</Description>");
                            returnMe.Append("<Order>" + objective.order + "</Order>");
                            returnMe.Append("</Objective>");
                        }
                        returnMe.Append("</Objectives>");
                        
                        
                        

                        returnMe.Append("</Course>");
                    }

                    returnMe.Append("</ReportingPeriod>");
                }

                /*
                returnMe.Append("<Course>");
                returnMe.Append("<Name>" + course.name + "</Name>");
                returnMe.Append("<CourseID>" + course.courseid + "</CourseID>");
                returnMe.Append("<ClassID>" + course.classid + "</ClassID>");
                returnMe.Append("<HasOutcomes>" + course.hasObjectives() + "</HasOutcomes>");
                returnMe.Append("<Teacher>");
                returnMe.Append("<GivenName>" + course.teacherFirstName + "</GivenName>");
                returnMe.Append("<Surname>" + course.teacherLastName + "</Surname>");
                returnMe.Append("<Title>" + course.teacherTitle + "</Title>");
                returnMe.Append("</Teacher>");
                returnMe.Append("</Course>");

                
                
                returnMe.Append("<AlphaMark>");
                returnMe.Append("</AlphaMark>");
                returnMe.Append("<PercentMark>");
                returnMe.Append("</PercentMark>");
                returnMe.Append("<Outcomes>");
                returnMe.Append("</Outcomes>");
                returnMe.Append("<LifeSkills>");
                returnMe.Append("</LifeSkills>");
                returnMe.Append("</ReportingPeriod>");
                */
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

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            
            selectedStudent = null;
            selectedSchool = null;
            selectedReportPeriod = null;
            studentMarks = null;

            if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
            {
                int studentID = -1;
                if (int.TryParse(Request.QueryString["studentid"], out studentID))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        selectedStudent = Student.loadThisStudent(connection, studentID.ToString());
                        selectedSchool = School.loadThisSchool(connection, int.Parse(selectedStudent.getSchoolID()));

                        /* Get student track, and determine the terms and report periods */                        
                        selectedStudent.track = Track.loadThisTrack(connection, int.Parse(selectedStudent.getTrackID()));

                        /* Populate the track with terms */
                        selectedStudent.track.terms = Term.loadTermsFromThisTrack(connection, selectedStudent.track);

                        /* Populate the terms with report periods */
                        foreach (Term t in selectedStudent.track.terms)
                        {
                            List<ObjectiveMark> TermObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisStudent(connection, t, selectedStudent);

                            t.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection,t);
                            
                            /* Load marks into the report period */
                            foreach (ReportPeriod r in t.ReportPeriods)
                            {                                
                                r.marks = Mark.loadMarksFromThisReportPeriod(connection, r, selectedStudent);
                                /* Translate the loaded marks into courses and marks */
                                 
                               
                                /* Collect a list of courses */                                
                                Dictionary<int, Course> allcourses = new Dictionary<int, Course>();
                                foreach (Mark m in r.marks)
                                {
                                    if (!allcourses.ContainsKey(m.courseID))
                                    {
                                        allcourses.Add(m.courseID, new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                    }
                                }
                                
                                
                                foreach (KeyValuePair<int, Course> kvp in allcourses)
                                {
                                    r.courses.Add(kvp.Value);
                                }

                                
                                foreach (Course c in r.courses)
                                {
                                    foreach (Mark m in r.marks)
                                    {                                
                                        if (m.courseID == c.courseid)
                                        {
                                            c.Marks.Add(m);
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
                        }
                    }
                }
            }

            if (selectedStudent != null)
            {
                //Response.Clear();                
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