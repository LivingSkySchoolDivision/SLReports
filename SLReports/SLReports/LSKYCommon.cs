using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SLReports
{
    public static class LSKYCommon
    {
        private static Random random = new Random(DateTime.Now.Millisecond); // Not cryptographically random, but random enough for what I need it for

        public static string internal_api_key = "6b05cb5705c07a4ca23a6bba779263ab983a5ae2";
        public static string userGroupName = "SchoolLogicDataExplorerUsers";
        public static string adminGroupName = "SchoolLogicDataExplorerAdmins";

        public static string dbConnectionString_SchoolLogic = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
        public static string dbConnectionString_OldSchoolLogic = ConfigurationManager.ConnectionStrings["SchoolLogic2013"].ConnectionString;
        public static string dbConnectionString_DataExplorer = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;
        public static string dbConnectionString_SchoolLogicTest = ConfigurationManager.ConnectionStrings["SchoolLogicTestDatabase"].ConnectionString;

        public static string boolToTrueFalse(bool thisBool)
        {
            if (thisBool)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }

        public static string boolToYesOrNoHTML(bool thisBool)
        {
            if (thisBool)
            {
                return "<span style=\"color: #007700;\">Yes</span>";
            }
            else
            {
                return "<span style=\"color: #770000;\">No</span>";
            }
        }

        public static string boolToYesOrNo(bool thisBool)
        {
            if (thisBool)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public static int boolToOneOrZero(bool thisBool)
        {
            if (thisBool)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static string removeSpaces(string working)
        {
            try
            {
                return Regex.Replace(working, @"[^\w]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        public static float translatePercentToOutcome(float percent)
        {
            float returnMe = -1;            

            if ((percent <= 100) && (percent >= 0))
            {
                if (percent < 40)
                {
                    returnMe = 1f;
                }
                else if (percent < 50)
                {
                    returnMe = 1.5f;
                }
                else if (percent < 60)
                {
                    returnMe = 2f;
                }
                else if (percent < 75)
                {
                    returnMe = 2.5f;
                }
                else if (percent < 90)
                {
                    returnMe = 3f;
                }
                else if (percent < 95)
                {
                    returnMe = 3.5f;
                }
                else if (percent <= 100)
                {
                    returnMe = 4f;
                }
            }

            return returnMe;
        }

        public static string getCurrentTimeStampForFilename()
        {
            return DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        }

        
        /// <summary>
        /// Loads mark data for a student for report cards. Loads terms, report periods, enrolled classes, class marks, outcomes, outcome marks, and attendance
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="thisStudent">The student to load data for</param>
        /// <param name="theseReportPeriods">Report periods to load data for</param>
        /// <returns>Student object loaded with terms, report periods, enrolled classes, class marks, outcomes, outcome marks, and attendance</returns>
        public static Student loadStudentMarkData(SqlConnection connection, Student thisStudent, List<ReportPeriod> theseReportPeriods)
        {
            Student returnedStudent = thisStudent;

            // **************************************************************
            // * Report Period Comments
            // **************************************************************

            // Load report period comments for the selected report periods
            List<ReportPeriodComment> allRPComments = ReportPeriodComment.loadRPCommentsForStudent(connection, thisStudent.getStudentID());
            List<ReportPeriodComment> filteredRPComments = ReportPeriodComment.getCommentsForTheseReportPeriods(allRPComments, theseReportPeriods);
            returnedStudent.ReportPeriodComments = filteredRPComments;

            // **************************************************************
            // * Report Periods
            // **************************************************************
            // Find the earliest report period and the last report period, for attendance dates
            DateTime earliestDate = DateTime.MaxValue;
            DateTime lastDate = DateTime.MinValue;

            List<int> detectedTermIDs = new List<int>();
            List<int> selectedReportPeriodIDs = new List<int>();
            List<ReportPeriod> reportPeriods = new List<ReportPeriod>();

            // Validate report periods so we aren't working with nulls
            foreach (ReportPeriod rp in theseReportPeriods)
            {
                if (rp != null)
                {
                    reportPeriods.Add(rp);
                }
            }
            
            foreach (ReportPeriod rp in reportPeriods)
            {
                // Create a list of loaded report period IDs for later use
                if (!selectedReportPeriodIDs.Contains(rp.ID))
                {
                    selectedReportPeriodIDs.Add(rp.ID);
                }

                // Find the earliest report period and the last report period, for attendance dates
                if (rp.startDate < earliestDate)
                {
                    earliestDate = rp.startDate;
                }

                if (rp.endDate > lastDate)
                {
                    lastDate = rp.endDate;
                }

                // Derive some terms from the given report periods while we are cycling through them
                if (!detectedTermIDs.Contains(rp.termID))
                {
                    detectedTermIDs.Add(rp.termID);
                }
                
            }

            // **************************************************************
            // * Terms
            // **************************************************************
            // Derive some terms from the given report periods while we are cycling through them
            List<Term> detectedTerms = new List<Term>();
            foreach (int termid in detectedTermIDs)
            {
                detectedTerms.Add(Term.loadThisTerm(connection, termid));
            }

            
            foreach (Term term in detectedTerms)
            {
                foreach (ReportPeriod rp in reportPeriods)
                {
                    // Put report periods into their respective terms
                    if (rp.termID == term.ID)
                    {
                        term.ReportPeriods.Add(rp);
                    }

                    // Determine the final report period in a term, and assign it appropriately if it was loaded
                    if (
                        (rp.endDate.Year == term.endDate.Year) &&
                        (rp.endDate.Month == term.endDate.Month) &&
                        (rp.endDate.Day == term.endDate.Day)
                        )
                    {
                        term.FinalReportPeriod = rp;
                    }
                }
            } 
            

            if (returnedStudent != null)
            {
                returnedStudent.school = School.loadThisSchool(connection, thisStudent.getSchoolIDAsInt());
                returnedStudent.track = Track.loadThisTrack(connection, returnedStudent.getTrackID());
                returnedStudent.track.terms = detectedTerms;

                // **************************************************************
                // * Attendance
                // **************************************************************
                // Load this student's attendance
                returnedStudent.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, thisStudent, earliestDate, lastDate);
                
                foreach (Term thisTerm in returnedStudent.track.terms)
                {
                    // **************************************************************
                    // * Enrolled Classes
                    // **************************************************************
                    // Load classes this student is enrolled in
                    thisTerm.Courses = SchoolClass.loadStudentEnrolledClassesForThisTerm(connection, returnedStudent, thisTerm);

                    foreach (SchoolClass thisClass in thisTerm.Courses)
                    {
                        thisClass.term = thisTerm;

                        // Put list of report periods into each class so we can easily reference it later
                        thisClass.ReportPeriods = reportPeriods;

                        // **************************************************************
                        // * Outcomes and Life Skills (or SLBs / Successful Learner Behaviors)
                        // **************************************************************
                        string lifeSkillsCategoryName = "Successful Learner Behaviours";
                                                    
                        //List<Outcome> classLifeSkills = Outcome.loadObjectivesForThisCourseByCategoryName(connection, thisClass, lifeSkillsCategoryName);
                        
                        List<OutcomeMark> classOutcomeMarks_All = OutcomeMark.loadOutcomeMarksForThisCourse(connection, thisTerm, returnedStudent, thisClass);
                        
                        // Split marks into life skills and not life skills
                        List<Outcome> classNormalOutcomes = new List<Outcome>();
                        List<Outcome> classLifeSkills = new List<Outcome>();

                        List<OutcomeMark> classMarks_LifeSkills = new List<OutcomeMark>();
                        List<OutcomeMark> classMarks_Outcomes = new List<OutcomeMark>();                        
                        
                        foreach (OutcomeMark om in classOutcomeMarks_All)
                        {
                            // Associate some of the loaded report period objects with the outcome marks                            
                            foreach (ReportPeriod rp in reportPeriods)
                            {
                                if (rp.ID == om.reportPeriodID)
                                {
                                    om.reportPeriod = rp;
                                }
                            }
                            
                            // Split marks into life skills and not life skills lists
                            if (om.outcome.category == lifeSkillsCategoryName)
                            {
                                classMarks_LifeSkills.Add(om);

                                classLifeSkills.Add(om.outcome);
                            }
                            else
                            {
                                classMarks_Outcomes.Add(om);

                                // While we are iterating through the list, derive a list of outcomes from the outcome marks, since we didn't load one earlier
                                classNormalOutcomes.Add(om.outcome);
                            }                            
                        }

                         
                        // Put the outcomes and outcome marks into the course object
                        thisClass.Outcomes = classNormalOutcomes;
                        thisClass.LifeSkills = classLifeSkills;
                        thisClass.OutcomeMarks = classMarks_Outcomes;                        
                        thisClass.LifeSkillMarks = classMarks_LifeSkills;                        
                    }

                    // **************************************************************
                    // * Class marks and comments
                    // **************************************************************
                    List<Mark> allMarks = new List<Mark>();
                    foreach (ReportPeriod thisReportPeriod in thisTerm.ReportPeriods)
                    {
                        thisReportPeriod.marks = Mark.loadMarksFromThisReportPeriod(connection, thisReportPeriod, returnedStudent);
                        foreach (Mark m in thisReportPeriod.marks)
                        {
                            allMarks.Add(m);
                        }
                    }

                    foreach (Mark m in allMarks)
                    {
                        foreach (SchoolClass c in thisTerm.Courses)
                        {
                            if (m.classID == c.classid)
                            {
                                c.Marks.Add(m);
                            }
                        }
                    }
                }
            }

            return returnedStudent;
        }

        public static int parseInt(string thisString)
        {
            int returnMe = -1;

            if (int.TryParse(thisString, out returnMe))
            {
                return returnMe;
            }

            return -1;

        }

        public static string getMonthName(int monthNum)
        {
            string returnMe = "Unknown";
            switch (monthNum)
            {
                case 1:
                    returnMe = "January";
                    break;
                case 2:
                    returnMe = "February";
                    break;
                case 3:
                    returnMe = "March";
                    break;
                case 4:
                    returnMe = "April";
                    break;
                case 5:
                    returnMe = "May";
                    break;
                case 6:
                    returnMe = "June";
                    break;
                case 7:
                    returnMe = "July";
                    break;
                case 8:
                    returnMe = "August";
                    break;
                case 9:
                    returnMe = "September";
                    break;
                case 10:
                    returnMe = "October";
                    break;
                case 11:
                    returnMe = "November";
                    break;
                case 12:
                    returnMe = "December";
                    break;
            }
            return returnMe;
        }

        public static List<String> getGroupMembers(string domain, string groupName)
        {
            List<string> returnMe = new List<string>();

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                using (GroupPrincipal grp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName))
                {
                    if (grp != null)
                    {
                        foreach (Principal p in grp.GetMembers(true))
                        {
                            returnMe.Add(p.SamAccountName);
                        }
                    }
                }
            }
            return returnMe;
        }

        public static string translateLocalURL(string path)
        {
            string path_working = path;

            if (path.Length > 0)
            {
                if (path[0] == '/')
                {
                    path_working = path.Substring(1, path.Length - 1);
                }
            }

            return "/SLReports/" + path_working;
        }

        public static string getServerName(HttpRequest Request)
        {
            return Request.ServerVariables["SERVER_NAME"].ToString().Trim();
        }

        public static string getServerURLPath(HttpRequest Request)
        {
            StringBuilder scriptPath = new StringBuilder();
            scriptPath.Append("https://");
            scriptPath.Append(Request.ServerVariables["SERVER_NAME"]);

            

            string[] working = Request.ServerVariables["SCRIPT_NAME"].Split('/');

            scriptPath.Append('/');
            scriptPath.Append(working[1]);
            scriptPath.Append('/');
            
            return scriptPath.ToString();
        }        

        public static string getRandomLipsumString()
        {
            string[] lipsumStrings = {
                                     "Duis scelerisque lectus sed tellus suscipit pulvinar. Nunc ullamcorper pretium blandit",
                                     "Praesent et pretium justo. Nulla faucibus nunc orci, non tristique velit consequat a",
                                     "Suspendisse placerat pulvinar tortor, at rhoncus nisl malesuada vitae",
                                     "Morbi viverra volutpat sem, vitae feugiat leo elementum sit amet. Vestibulum diam ipsum, condimentum a posuere quis, venenatis et justo",
                                     "Donec euismod sagittis dolor ut venenatis",
                                     "Phasellus auctor, velit a mollis placerat, sem erat facilisis nisi, sit amet fringilla quam nulla vitae ipsum",
                                     "Praesent sollicitudin pellentesque mi vestibulum cursus",
                                     "Nulla sollicitudin ante vitae libero pulvinar consectetur. Nunc dignissim sed odio at molestie",
                                     "Duis eu libero sapien. Pellentesque pretium malesuada purus. Sed nec augue in nibh porta bibendum",
                                     "Cras commodo lorem sed velit feugiat, eget ultrices odio luctus. Proin congue porttitor elit ac elementum. Etiam quis urna mattis, fermentum magna nec, cursus neque",
                                     "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                                     "Integer ultrices magna vitae risus varius dictum.",
                                     "Aenean aliquam dolor nec aliquet mollis. ",
                                     "Sed varius, neque sed iaculis viverra, nisi urna sagittis orci, a congue arcu magna a nisi."
                                 };

            return lipsumStrings[random.Next(0,lipsumStrings.Length - 1)];

        }

    }
}