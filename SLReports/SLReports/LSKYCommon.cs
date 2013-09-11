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

        public static string findClassNameForThisBlock(List<SchoolClass> classes, string block)
        {
            /* Try to parse the block */

            List<string> classNames = new List<string>();


            int blockNum = -1;
            if (int.TryParse(block, out blockNum))
            {
                foreach (SchoolClass thisclass in classes)
                {
                    if (thisclass.blockNumber == blockNum)
                    {
                        classNames.Add(thisclass.name);
                    }
                }
            }


            StringBuilder returnMe = new StringBuilder();
            for (int x = 0; x < classNames.Count; x++)
            {
                returnMe.Append(classNames[x]);
                if (x < classNames.Count - 1)
                {
                    returnMe.Append(", ");
                }
            }

            return returnMe.ToString();
        }

        public static Student loadStudentMarkData(SqlConnection connection, Student thisStudent, List<ReportPeriod> theseReportPeriods)
        {
            Student returnedStudent = thisStudent;

            /* Find the earliest report period and the last report period, for attendance dates */
            DateTime earliestDate = DateTime.MaxValue;
            DateTime lastDate = DateTime.MinValue;

            List<int> detectedTermIDs = new List<int>();
            List<int> selectedReportPeriodIDs = new List<int>();
            List<ReportPeriod> reportPeriods = new List<ReportPeriod>();

            /* Validate report periods so we aren't working with nulls */
            foreach (ReportPeriod rp in theseReportPeriods)
            {
                if (rp != null)
                {
                    reportPeriods.Add(rp);
                }
            }
            
            foreach (ReportPeriod rp in reportPeriods)
            {                
                /* Create a list of loaded report period IDs for later use */
                if (!selectedReportPeriodIDs.Contains(rp.ID))
                {
                    selectedReportPeriodIDs.Add(rp.ID);
                }

                /* Find the earliest report period and the last report period, for attendance dates */
                if (rp.startDate < earliestDate)
                {
                    earliestDate = rp.startDate;
                }

                if (rp.endDate > lastDate)
                {
                    lastDate = rp.endDate;
                }

                /* Derive some terms from the given report periods while we are cycling through them */
                if (!detectedTermIDs.Contains(rp.termID))
                {
                    detectedTermIDs.Add(rp.termID);
                }
                
            }

            /* Derive some terms from the given report periods while we are cycling through them */
            List<Term> detectedTerms = new List<Term>();
            foreach (int termid in detectedTermIDs)
            {
                detectedTerms.Add(Term.loadThisTerm(connection, termid));
            }

            
            foreach (Term term in detectedTerms)
            {
                foreach (ReportPeriod rp in reportPeriods)
                {
                    /* Put report periods into their respective terms */
                    if (rp.termID == term.ID)
                    {
                        term.ReportPeriods.Add(rp);
                    }

                    /* Determine the final report period in a term, and assign it appropriately if it was loaded */
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

                /* Load attendance */
                returnedStudent.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, thisStudent, earliestDate, lastDate);
                
                foreach (Term thisTerm in returnedStudent.track.terms)
                {
                    /* Load enrolled classes */
                    thisTerm.Courses = SchoolClass.loadStudentEnrolledClassesForThisTerm(connection, returnedStudent, thisTerm);

                    foreach (SchoolClass thisClass in thisTerm.Courses)
                    {

                        thisClass.term = thisTerm;

                        /* Put list of report periods into each class so we can easily reference it later */
                        thisClass.ReportPeriods = reportPeriods;

                        /* Load objectives and objective marks */
                        thisClass.Outcomes = Outcome.loadObjectivesForThisCourse(connection, thisClass);

                        List<OutcomeMark> AllOutcomeMarksForThisCourse = OutcomeMark.loadObjectiveMarksForThisCourse(connection, thisTerm, returnedStudent, thisClass);

                        /* Filter out objectivemarks for report periods that we don't care about */
                        foreach (OutcomeMark om in AllOutcomeMarksForThisCourse) 
                        {
                            if (selectedReportPeriodIDs.Contains(om.reportPeriodID))
                            {
                                thisClass.OutcomeMarks.Add(om);
                            }                            
                        }

                        /* Put objective marks in the corresonding objective */
                        foreach (OutcomeMark objectivemark in thisClass.OutcomeMarks)
                        {
                            foreach (Outcome objective in thisClass.Outcomes)
                            {
                                if (objectivemark.objectiveID == objective.id)
                                {
                                    objectivemark.objective = objective;
                                }
                            }

                            foreach (ReportPeriod rp in reportPeriods)
                            {
                                if (rp.ID == objectivemark.reportPeriodID)
                                {
                                    objectivemark.reportPeriod = rp;
                                }
                            }

                        }

                        foreach (Outcome objective in thisClass.Outcomes)
                        {
                            foreach (OutcomeMark objectivemark in thisClass.OutcomeMarks)
                            {
                                if (objective.id == objectivemark.objectiveID)
                                {
                                    objective.marks.Add(objectivemark);
                                }
                            }
                        }

                    }

                    /* Load class marks */

                    List<Mark> allMarks = new List<Mark>();
                    //thisTerm.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, thisTerm.ID);

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

    }
}