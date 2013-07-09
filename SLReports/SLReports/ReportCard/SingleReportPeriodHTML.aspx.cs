using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class SingleReportPeriodHTML : System.Web.UI.Page
    {
        Student selectedStudent = null;
        ReportPeriod selectedReportPeriod = null;

        Random random;

        protected void Page_Init(object sender, EventArgs e)
        {
            random = new Random();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Info we need from the querystring:
             *  - Student ID number
             *  - Report Period ID number
             */

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
                {
                    int studentID = -1;
                    if (int.TryParse(Request.QueryString["studentid"], out studentID))
                    {
                        if (!String.IsNullOrEmpty(Request.QueryString["reportperiod"]))
                        {
                            int reportPeriodID = -1;
                            if (int.TryParse(Request.QueryString["reportperiod"], out reportPeriodID))
                            {
                                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                {
                                    selectedStudent = Student.loadThisStudent(connection, studentID.ToString());

                                    #region Load data for the student
                                    if (selectedStudent != null)
                                    {                                    
                                        selectedStudent.school = School.loadThisSchool(connection, int.Parse(selectedStudent.getSchoolID())); 

                                        /* Get student track, and determine the terms and report periods */
                                        /* TODO: This may now be redundant */
                                        selectedStudent.track = Track.loadThisTrack(connection, selectedStudent.getTrackID());

                                        /* Populate the track with terms */
                                        selectedStudent.track.terms = Term.loadTermsFromThisTrack(connection, selectedStudent.track);

                                        /* Populate the terms with report periods */
                                        foreach (Term t in selectedStudent.track.terms)
                                        {
                                            List<ObjectiveMark> TermObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisStudent(connection, t, selectedStudent);

                                            t.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, t);

                                            Dictionary<int, SchoolClass> termCourses = new Dictionary<int, SchoolClass>();
                                            termCourses.Clear();

                                            /* Load marks into the report period */
                                            foreach (ReportPeriod r in t.ReportPeriods)
                                            {
                                                if (r.ID == reportPeriodID)
                                                {
                                                    r.marks = Mark.loadMarksFromThisReportPeriod(connection, r, selectedStudent);
                                                    selectedReportPeriod = r;

                                                    Dictionary<int, SchoolClass> allcourses = new Dictionary<int, SchoolClass>();
                                                    foreach (Mark m in r.marks)
                                                    {
                                                        if (!allcourses.ContainsKey(m.courseID))
                                                        {
                                                            allcourses.Add(m.courseID, new SchoolClass(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                                        }

                                                        if (!termCourses.ContainsKey(m.courseID))
                                                        {
                                                            termCourses.Add(m.courseID, new SchoolClass(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                                        }
                                                    }


                                                    foreach (KeyValuePair<int, SchoolClass> kvp in termCourses)
                                                    {
                                                        SchoolClass c = kvp.Value;
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
                                                        c.Objectives = Objective.loadObjectivesForThisCourse(connection, c);
                                                        
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

                                                    foreach (KeyValuePair<int, SchoolClass> kvp in termCourses)
                                                    {
                                                        t.Courses.Add(kvp.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Display the content
                                    litObjectiveKey.Text = generateObjectiveKey();
                                    litNamePlate.Text = generateStudentNameplate(selectedStudent);
                                    litMarks.Text = generateMarkTable(selectedStudent);
                                    #endregion
                                }
                            }
                            else
                            {
                                Response.Write("Invalid or missing report period ID");
                            }
                        }
                        else
                        {
                            Response.Write("Invalid or missing report period ID");
                        }

                    }
                    else
                    {
                        Response.Write("Invalid or missing student ID");
                    }
                }
                else
                {
                    Response.Write("Invalid or missing student ID");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<BR>" + ex.StackTrace);
            }

        }

        protected string generateStudentNameplate(Student student)
        {            
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append("<div style=\"width: 500px;margin-left: auto; margin-right: auto;\">"); 

            returnMe.Append("<table width=\"500\">");
            returnMe.Append("<tr><td>");           
            if (student.hasPhoto())
            {
                returnMe.Append("<img width=\"137\" height=\"175\" style=\"border: 1px solid black;\" src=\"/SLReports/Photos/GetPhoto.aspx?studentnumber=" + student.getStudentID() + "\">");
            }

            returnMe.Append("</td><td valign=\"top\">");

            returnMe.Append("<table width=\"500\">");
            returnMe.Append("<tr>");
            returnMe.Append("<td valign=\"top\" width=\"125\"><b>Student</b></td><td valign=\"top\"><div class=\"rc_studentname\">" + student.getDisplayName() + "</div></td></tr>");
            returnMe.Append("<tr><td valign=\"top\"><b>Student Number</b></td><td valign=\"top\">" + student.getStudentID() + "</tr></td>");
            returnMe.Append("<td valign=\"top\" width=\"125\"><b>School</b></td><td valign=\"top\">" + student.getSchoolName() + "</td></tr>");
            if (!string.IsNullOrEmpty(student.getHomeRoom()))
            {
                returnMe.Append("<tr><td><b>Homeroom</b></td><td valign=\"top\">" + student.getHomeRoom() + "</tr></td>");
            }
            returnMe.Append("<tr><td valign=\"top\"><b>Report Period</b></td><td valign=\"top\">" + selectedReportPeriod.name + " <i>(" + selectedReportPeriod.startDate.ToLongDateString() + " to " + selectedReportPeriod.endDate.ToLongDateString() + ")</i></tr></td>");            
            returnMe.Append("</table>");
            

            


            returnMe.Append("</td></tr>"); 
            returnMe.Append("</table>");
            returnMe.Append("</div><br/>");


            return returnMe.ToString();
        }
        protected string generateStudentNameplate_Dummy(Student student)
        {
            StringBuilder returnMe = new StringBuilder();

            

            returnMe.Append("<table width=\"100%\">");
            returnMe.Append("<tr><td width=64><img src=\"/SLReports/Logo_Circle_Notext_Trans.png\" style=\"width: 64px; height: 64px;\">");
            returnMe.Append("</td><td><div style=\"font-size: 20pt;font-weight: bold;\">Living Sky School Division No. 202</div><div style=\"font-weight: bold; font-size: 16pt;\">"+student.getSchoolName()+"</div>");
            returnMe.Append("</td></tr>");
            returnMe.Append("</table><br/><br/><br/><br/><br/>");

            returnMe.Append("<div style=\"width: 500px;margin-left: auto; margin-right: auto;\">");
            returnMe.Append("<table width=\"500\">");
            returnMe.Append("<tr><td>");
            if (student.hasPhoto())
            {
                returnMe.Append("<img width=\"137\" height=\"175\" style=\"border: 1px solid black;\" src=\"/SLReports/student_dummy_photo.jpg\">");
            }

            returnMe.Append("</td><td valign=\"top\">");

            returnMe.Append("<table width=\"500\">");
            returnMe.Append("<tr>");
            returnMe.Append("<td valign=\"top\" width=\"125\"><b>Student</b></td><td valign=\"top\"><div class=\"rc_studentname\">Bill Gates</div></td></tr>");
            returnMe.Append("<tr><td valign=\"top\"><b>Student Number</b></td><td valign=\"top\">000000000</tr></td>");
            returnMe.Append("<td valign=\"top\" width=\"125\"><b>School</b></td><td valign=\"top\"><b>Example report card - First draft - Work in progress - Multi report period example</b></td></tr>");
            if (!string.IsNullOrEmpty(student.getHomeRoom()))
            {
                returnMe.Append("<tr><td><b>Homeroom</b></td><td valign=\"top\">" + student.getHomeRoom() + "</tr></td>");
            }
            returnMe.Append("<tr><td valign=\"top\"><b>Report Period</b></td><td valign=\"top\">" + selectedReportPeriod.name + " <i>(" + selectedReportPeriod.startDate.ToLongDateString() + " to " + selectedReportPeriod.endDate.ToLongDateString() + ")</i></tr></td>");            
            returnMe.Append("</table>");





            returnMe.Append("</td></tr>");
            returnMe.Append("</table>");
            returnMe.Append("</div><br/><br/><br/><br/><br/><br/>");


            return returnMe.ToString();
        }

        public string getMD5(string input)
        {
            string returnMe = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }


        }

        static int canvasCounter = 0;

        protected string generateObjectiveKey()
        {
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append("<br/><br/><table cellpadding=3 style=\"margin-left: auto; margin-right: auto;\">");
            returnMe.Append("<tr><td>" + generateOutcomeProgressBar("4") + "</td><td><b>4: Master</b> Insightful understanding of the outcome</td></tr>");
            returnMe.Append("<tr><td>" + generateOutcomeProgressBar("3") + "</td><td><b>3: Proficient</b> A well developed understanding</td></tr>");
            returnMe.Append("<tr><td>" + generateOutcomeProgressBar("2") + "</td><td><b>2: Approaching</b> A basic understanding</td></tr>");
            returnMe.Append("<tr><td>" + generateOutcomeProgressBar("1") + "</td><td><b>1: Beginning</b> A partial understanding</td></tr>");
            returnMe.Append("<tr><td>" + generateOutcomeProgressBar("IE") + "</td><td><b>IE:</b> Insufficient Evidence Submitted</td></tr>");
            returnMe.Append("</table><br/><br/>");
            return returnMe.ToString();

        }

        protected string generateOutcomeNumberBar()
        {
            int barWidth = 125;
            int barHeight = 16;
            canvasCounter++;
            String barID = "outcomeBar_" + canvasCounter + "_" + getMD5(DateTime.Now.Millisecond.ToString());

            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("<canvas style=\"width: "+barWidth+"px;height: "+barHeight+"px;\" id=\"" + barID + "\" width=\"" + barWidth + "\" height=\"" + barHeight + "\">Number Bar</canvas>");

            returnMe.Append("<script>createNumberBar('" + barID + "');</script>");

            return returnMe.ToString();
        }

        protected string generateOutcomeProgressBar(string value)
        {
            int barWidth = 150;
            int barHeight = 16;
            canvasCounter++;
            String barID = "outcomeBar_" + canvasCounter + "_" + getMD5(DateTime.Now.Millisecond.ToString());

            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("<canvas id=\"" + barID + "\" width=\"" + barWidth + "\" height=\"" + barHeight + "\">" + value + "</canvas>");

            double parsedValue = -1;

            if (double.TryParse(value, out parsedValue))
            {
                returnMe.Append("<script>createOutcomeBar('" + barID + "','" + value + "');</script>");
            }
            else
            {
                if (value.ToLower() == "ie")
                {
                    /* Display "IE" in the box */
                    returnMe.Append("<script>createOutcomeBarIE('" + barID + "');</script>");
                }
            }

            return returnMe.ToString();
        }

        protected string generateOutcomeProgressBarThin(string value)
        {
            int barWidth = 125;
            int barHeight = 10;
            canvasCounter++;
            String barID = "outcomeBar_" + canvasCounter + "_" + getMD5(DateTime.Now.Millisecond.ToString());

            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("<canvas id=\"" + barID + "\" width=\"" + barWidth + "\" height=\"" + barHeight + "\">" + value + "</canvas>");

            double parsedValue = -1;

            if (double.TryParse(value, out parsedValue))
            {
                returnMe.Append("<script>createOutcomeBar_Thin('" + barID + "','" + value + "');</script>");
            }
            else
            {
                if (value.ToLower() == "ie")
                {
                    /* Display "IE" in the box */
                    returnMe.Append("<script>createOutcomeBarIE('" + barID + "');</script>");
                }
            }

            return returnMe.ToString();
        }

        protected string generateMarkTable_old(Student student)
        {
            StringBuilder returnMe = new StringBuilder();


            returnMe.Append("<div><b>Report Period:</b> " + selectedReportPeriod.name + " (" + selectedReportPeriod.startDate.ToShortDateString() + " to " + selectedReportPeriod.endDate.ToShortDateString() + ")</div><br/><br/>");

            foreach (Term term in student.track.terms)
            {
                foreach (SchoolClass course in term.Courses)
                {
                    returnMe.Append("<table border=0 width=\"100%\">");
                    returnMe.Append("<tr>");

                    returnMe.Append("<td><div class=\"course_name\"><b>" + course.name + "</b> - " + course.teacherName + "</div></td>");
                    returnMe.Append("<td align=\"right\"><div class=\"course_name\"><b>");
                    if (!course.hasObjectives())
                    {
                        foreach (Mark mark in course.Marks)
                        {
                            if (double.Parse(mark.numberMark) > 0)
                            {
                                returnMe.Append(mark.numberMark + "%");
                            }
                        }
                    }
                    returnMe.Append("</b></div></td>");


                    returnMe.Append("</tr>");

                    returnMe.Append("<tr>");
                    returnMe.Append("<td width=\"75%\" valign=\"top\">"); /* Outcomes */
                    if (course.hasObjectives())
                    {
                        returnMe.Append("<div><b>Outcomes:</b></div>");
                        returnMe.Append("<table border=0 cellspacing=5 width=\"100%\">");

                        foreach (ReportPeriod reportPeriod in course.ReportPeriods)
                        {
                            foreach (ObjectiveMark objectivem in course.ObjectiveMarks)
                            {
                                if ((objectivem.courseID == course.courseid) && (objectivem.reportPeriodID == reportPeriod.ID))
                                {
                                    returnMe.Append("<tr>");
                                    returnMe.Append("<td><div class=\"rc_objectivetext\">" + objectivem.description + "</div></td>");
                                    returnMe.Append("<td valign=\"top\" width=100>" + generateOutcomeProgressBar(objectivem.mark) + "</td>");
                                    returnMe.Append("</tr>");
                                }
                            }
                        }



                        returnMe.Append("</table>");

                        returnMe.Append("</td><td width=\"25%\" valign=\"top\">");   /* Life skills */

                        /* Ipsum life skills, because there are no life skills yet */
                        returnMe.Append("<table border=0 width=\"100%\">");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" ><b>Life Skills:</b></td>");
                        returnMe.Append("<td valign=\"top\" width=100>" + generateOutcomeNumberBar() + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" ><div class=\"\">Engagement</div></td>");
                        returnMe.Append("<td valign=\"top\"  width=100>" + generateOutcomeProgressBarThin("4") + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" >Citizenship</td>");
                        returnMe.Append("<td valign=\"top\"  width=100>" + generateOutcomeProgressBarThin("3.25") + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" >Collaboration</td>");
                        returnMe.Append("<td valign=\"top\"  width=100>" + generateOutcomeProgressBarThin("2.25") + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" >Leadership</td>");
                        returnMe.Append("<td valign=\"top\"  width=100>" + generateOutcomeProgressBarThin("1.25") + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("<tr>");
                        returnMe.Append("<td valign=\"top\" >Self-Direction</td>");
                        returnMe.Append("<td valign=\"top\"  width=100>" + generateOutcomeProgressBarThin("0.5") + "</td>");
                        returnMe.Append("</tr>");
                        returnMe.Append("</table>");

                        returnMe.Append("</td>");
                        returnMe.Append("</tr>");

                    }
                    returnMe.Append("<tr><td colspan=2>");
                    foreach (Mark mark in course.Marks)
                    {
                        if (!string.IsNullOrEmpty(mark.comment))
                        {
                            //returnMe.Append("<div><b>Comments:</b><br><div style=\"padding-left: 10px;\"> " + mark.comment + "</div></div>");
                            returnMe.Append("<div><b>Comments:</b><br><div style=\"padding-left: 10px;\">Cras dictum lacinia tellus, sed rhoncus nulla lacinia in. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus diam elit, vehicula vitae placerat et, malesuada quis ante.</div></div>");
                        
                        }
                    }
                    returnMe.Append("</td></tr>");


                    returnMe.Append("</table><br/><br/><br/><br/>");
                }
            }
            return returnMe.ToString();
        }
        protected string generateMarkTable(Student student)
        {
            StringBuilder returnMe = new StringBuilder();

            foreach (Term term in student.track.terms)
            {
                foreach (SchoolClass course in term.Courses)
                {
                    returnMe.Append("<table border=0 cellpadding=5 width=\"100%\">");
                    returnMe.Append("<tr>");

                    returnMe.Append("<td><div class=\"course_name\"><b>" + course.name + "</b> - " + course.teacherName + "</div></td>");
                    returnMe.Append("<td align=\"right\"><div class=\"course_name\"><b>");
                    if (!course.hasObjectives())
                    {
                        foreach (Mark mark in course.Marks)
                        {
                            if (double.Parse(mark.numberMark) > 0)
                            {
                                returnMe.Append(mark.numberMark + "%");
                            }
                        }
                    }
                    returnMe.Append("</b></div></td>");


                    returnMe.Append("</tr>");


                    /* Outcomes */
                    if (course.hasObjectives())
                    {
                        returnMe.Append("<tr>");
                        returnMe.Append("<td colspan=2 width=\"75%\" valign=\"top\">");

                        returnMe.Append("<div><b>Outcomes:</b></div>");
                        returnMe.Append("<table border=0 cellspacing=5 width=\"100%\">");

                        foreach (ReportPeriod reportPeriod in course.ReportPeriods)
                        {
                            foreach (ObjectiveMark objectivem in course.ObjectiveMarks)
                            {
                                if ((objectivem.courseID == course.courseid) && (objectivem.reportPeriodID == reportPeriod.ID))
                                {
                                    returnMe.Append("<tr>");
                                    returnMe.Append("<td><div class=\"rc_objectivetext\">" + objectivem.description + "</div></td>");
                                    returnMe.Append("<td valign=\"top\" width=100>" + generateOutcomeProgressBar(objectivem.mark) + "</td>");
                                    returnMe.Append("</tr>");
                                }
                            }
                        }
                        returnMe.Append("</table>");



                        returnMe.Append("</td>");
                        returnMe.Append("</tr>");

                        /* Life skills */
                        /* Ipsum life skills, because there are no life skills yet */
                        returnMe.Append("<tr><td colspan=2 valign=\"top\">");
                        returnMe.Append("<div><b>Life Skills: </b></div>");
                        returnMe.Append("<table border=0  width=\"100%\">");
                        returnMe.Append("<tr>");
                        //returnMe.Append("<td align=\"Left\" valign=\"top\"><b>Report Period</b></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\"><b>Engagement</b></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\"><b>Citizenship</b></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\"><b>Collaboration</b></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\"><b>Leadership</b></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\"><b>Self-Direction</b></td>");
                        returnMe.Append("</tr>");


                        /*
                        returnMe.Append("<tr>");
                        returnMe.Append("<td align=\"Left\" valign=\"top\"></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeNumberBar() + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeNumberBar() + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeNumberBar() + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeNumberBar() + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeNumberBar() + "</td>"); 
                        returnMe.Append("</tr>");
                        */

                        /* Additional rows for additional report periods */

                        returnMe.Append("<tr>");

                        //returnMe.Append("<td align=\"Left\" valign=\"top\"><div style=\"font-size: 8pt;\">R1</div></td>");

                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBar(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBar(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBar(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBar(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBar(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("</tr>");

                        /*
                        returnMe.Append("<tr>"); 
                        returnMe.Append("<td align=\"Left\" valign=\"top\"><div style=\"font-size: 8pt;\">R2</div></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("</tr>"); returnMe.Append("<tr>");

                        returnMe.Append("<tr>");
                        returnMe.Append("<td align=\"Left\" valign=\"top\"><div style=\"font-size: 8pt;\">R3</div></td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("<td align=\"center\" valign=\"top\">" + generateOutcomeProgressBarThin(GetRandomDouble(0, 4).ToString()) + "</td>");
                        returnMe.Append("</tr>");
                        */

                        returnMe.Append("</table>");



                        returnMe.Append("</td></tr>");


                    }
                    returnMe.Append("<tr><td colspan=2>");
                    foreach (Mark mark in course.Marks)
                    {
                        if (!string.IsNullOrEmpty(mark.comment))
                        {
                            returnMe.Append("<div><b>Comments:</b><br><div style=\"padding-left: 10px;\"> " + mark.comment + "</div></div>");
                            //returnMe.Append("<div><b>Comments:</b><br><div style=\"padding-left: 10px;\">Cras dictum lacinia tellus, sed rhoncus nulla lacinia in. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus diam elit, vehicula vitae placerat et, malesuada quis ante.</div></div>");
                        }
                    }
                    returnMe.Append("</td></tr>");


                    returnMe.Append("</table><br/><br/><br/>");
                }
            }
            return returnMe.ToString();
        }

        public double GetRandomDouble(double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

    }
}